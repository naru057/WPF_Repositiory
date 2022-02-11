using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCalculator.WindowControlManager
{
	class WindowControlManager : IDialogService
	{
		private List<Type> _dialogTypes = new();
		private List<IDialog> _openedDialogs = new();
		public void Register<TDialog>() where TDialog : class, IDialog
		{
			var type = typeof(TDialog);
			if (_dialogTypes.Any(d => d.Equals(type)))
			{
				return;
			}
			_dialogTypes.Add(type);
		}

		public void Set<TContext>(TContext context) where TContext : IContext
		{
			this.Set((t)=> true, context);
		}

		public void Set<TContext, TDialog>(TContext context)
			where TContext : IContext
			where TDialog : IDialog
		{
			var type=typeof(TDialog);
			this.Set((t) => t.Equals(type), context);
		}
		private bool? Set<TContext>(Func<Type, bool> isExist, TContext context, bool isModal=false) where TContext : IContext
		{
			if (!_dialogTypes.Any(isExist))
			{
				throw new NotImplementedException("not matched dialog type");
			}

			var dialogType = _dialogTypes.First(isExist);
			var dialog = Activator.CreateInstance(dialogType) as IDialog;

			var dataContext = dialog.Datacontext as IDialogContext;
			dataContext.Context = context;

			_openedDialogs.Add(dialog);

			if (!isModal)
			{
				dialog.Show();

				return null;
			}

			return dialog.ShowDialog();
		}
		public void Clear()
		{
			_openedDialogs.ForEach(d =>
			{
				try
				{
					d.Close();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			});
			_openedDialogs.Clear();
			_dialogTypes.Clear();
		}

		public void Dispose()
		{
			this.Clear();
		}

		public void Out<TContext>(TContext context) where TContext : IContext
		{
			Out(d =>
			{
				var datacontext = d.Datacontext as IDialogContext;
				return datacontext.Context.Equals(context);
			});
		}

		public void Out<TContext, TDialog>(TContext context)
			where TContext : IContext
			where TDialog : IDialog
		{
			Out(d =>
			{
				var dialogType = d.GetType();
				return dialogType.Equals(typeof(TDialog));
			});
		}

		private void Out(Func<IDialog, bool> isexist)
		{
			var opened = _openedDialogs.Where(isexist).ToList();
			try
			{
				foreach (var dialog in opened)
				{
					dialog.Close();
					dialog.Datacontext = null;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				foreach (var dialog in opened)
				{
					_openedDialogs.Remove(dialog);
				}
			}
		}

		public bool? SetAwait<TContext>(TContext context) where TContext : IContext
		{
			return this.Set((t) => true, context, true);
		}

		public bool? SetAwait<TContext, TDialog>(TContext context)
			where TContext : IContext
			where TDialog : IDialog
		{
			var type = typeof(TDialog);
			return Set((t) => t.Equals(type), context, true);
		}

		
	}
}
