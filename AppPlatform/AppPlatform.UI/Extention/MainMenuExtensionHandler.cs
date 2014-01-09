using MvcOSGi.Shell.Models;
using UIShell.Extension;

namespace MvcOSGi.Shell.Extension
{
    class MainMenuExtensionHandler : ExtensionChangeHandler
    {
        private readonly ApplicationViewModel _viewModel;

        public MainMenuExtensionHandler(ApplicationViewModel viewModel)
         {
             _viewModel = viewModel;
             Initialize(new MainMenuBuilder(), OnNewItem, OnRemoveItem);
         }

       private void OnNewItem(object item)
       {
           _viewModel.MainMenuItems.Add((MenuItem)item);
       }

       private void OnRemoveItem(object item)
       {
           _viewModel.MainMenuItems.Remove((MenuItem)item);
       }
    }

    class SidebarExtensionHandler : ExtensionChangeHandler
    {
        private readonly ApplicationViewModel _viewModel;

        public SidebarExtensionHandler(ApplicationViewModel viewModel)
        {
            _viewModel = viewModel;
            Initialize(new MainMenuBuilder(), OnNewItem, OnRemoveItem);
        }

        private void OnNewItem(object item)
        {
            _viewModel.SidebarMenuItems.Add((MenuItem)item);
        }

        private void OnRemoveItem(object item)
        {
            _viewModel.SidebarMenuItems.Remove((MenuItem)item);
        }
    }
}