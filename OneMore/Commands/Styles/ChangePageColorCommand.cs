﻿//************************************************************************************************
// Copyright © 2020 Steven M Cohn.  All rights reserved.
//************************************************************************************************

namespace River.OneMoreAddIn
{
	using River.OneMoreAddIn.Models;
	using System.Linq;
	using System.Windows.Forms;
	using System.Xml.Linq;


	internal class ChangePageColorCommand : Command
	{
		public ChangePageColorCommand()
		{
		}


		public void Execute()
		{
			logger.WriteLine($"{nameof(ChangePageColorCommand)}.Execute()");

			Page page = null;

			using (var manager = new ApplicationManager())
			{
				page = new Page(manager.CurrentPage());
			}

			using (var dialog = new Dialogs.ChangePageColorDialog(page.GetPageColor(out _, out _)))
			{
				if (dialog.ShowDialog(owner) == DialogResult.OK)
				{
					var element = page.Root
						.Elements(page.Namespace + "PageSettings")
						.FirstOrDefault();

					if (element != null)
					{
						var attr = element.Attribute("color");
						if (attr != null)
						{
							attr.Value = dialog.PageColor;
						}
						else
						{
							element.Add(new XAttribute("color", dialog.PageColor));
						}

						using (var manager = new ApplicationManager())
						{
							manager.UpdatePageContent(page.Root);
						}
					}
					else
					{
						logger.WriteLine("ChangePageColor failed because PageSettings not found");
					}
				}
			}
		}
	}
}
