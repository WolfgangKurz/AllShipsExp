using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;

namespace AllShipsExp
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ITool))]
	[ExportMetadata("Guid", "21090FFF-BDF1-4F40-9847-64C180337AEF")]
	[ExportMetadata("Title", "AllShipsExp")]
	[ExportMetadata("Description", "All Ship's Exp")]
	[ExportMetadata("Version", "1.0.0")]
	[ExportMetadata("Author", "BeerAdmiral")] // wolfgangkurzdev@gmail.com
	[ExportMetadata("AuthorURL", "http://swaytwig.com/")]
	public class AllShipsExp : IPlugin, ITool
	{
		private ToolViewModel viewModel;
		string ITool.Name => "AllShipsExp";
		object ITool.View => new ToolView { DataContext = this.viewModel };

		public void Initialize()
		{
			this.viewModel = new ToolViewModel();
		}
	}
}
