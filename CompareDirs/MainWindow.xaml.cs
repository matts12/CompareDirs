﻿using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompareDirs { 
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		//members
		private string path1, path2;
		private FolderBrowserDialog chooserDialog;
		//constructors
		public MainWindow() {
			InitializeComponent();
			path1 = path2 = null;
			chooserDialog = new FolderBrowserDialog();
			chooserDialog.ShowNewFolderButton = false;
		}
		//methods
		public void OpenChooser1Click(object sender, RoutedEventArgs e) {
			string s = OpenDialog();
			path1 = s != null ? s : path1;
			dirLabel1.Content = path1 == null ? "Select the first directory to compare ->" : path1;
		}
		public void OpenChooser2Click(object sender, RoutedEventArgs e) {
			string s = OpenDialog();
			path2 = s != null ? s : path2;
			dirLabel2.Content = path2 == null ? "Select the second directory to compare ->" : path2;
        }
		public string OpenDialog() {
			DialogResult res = chooserDialog.ShowDialog();
			return res.Equals(System.Windows.Forms.DialogResult.OK) ? chooserDialog.SelectedPath : null;
		}
		private void ConvertToVisualTree(System.Windows.Controls.TreeView visTree, TreeRoot root) {
			visTree.Items.Clear();
			TreeViewItem visRoot = CreateTreeViewItem(root.Change, root.Name);
			visTree.Items.Add(visRoot);
			foreach(Traversable t in root.Root.Children) {
				AddChildrenToVisualTree(visRoot, t);
			}
		}
		private void AddChildrenToVisualTree(TreeViewItem parent, Traversable level) {
			TreeViewItem vis = CreateTreeViewItem(level.Change, level.Name);
			parent.Items.Add(vis);
			if(level is FileTree) {
				foreach (Traversable t in (level as FileTree).Children) {
					AddChildrenToVisualTree(vis, t);
				}
			}
		}
		private TreeViewItem CreateTreeViewItem(Difference diff, string text) {
			StackPanel p = new StackPanel();
			p.Orientation = System.Windows.Controls.Orientation.Horizontal;
			Rectangle r = new Rectangle();
			r.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			double pixSize = (double)new LengthConverter().ConvertFrom("10px");
			r.Height = pixSize;
			r.Width = pixSize;
			switch (diff) {
				case Difference.SAME: r.Fill = Brushes.Bisque; break;
				case Difference.NEW: r.Fill = Brushes.Green; break;
				case Difference.MISSING: r.Fill = Brushes.Red; break;
				case Difference.BOTH: r.Fill = Brushes.DodgerBlue; break;
				default: r.Fill = Brushes.White; break;
			}
			p.Children.Add(r);

			System.Windows.Controls.Label l = new System.Windows.Controls.Label();
			l.Content = text;
			p.Children.Add(l);
			TreeViewItem tvi = new TreeViewItem();
			tvi.Header = p;
			return tvi;
		}
		public void CompareButtonClick(object sender, RoutedEventArgs e) {
			try {
				if (path1 != null && path2 != null) {
					TreeRoot root1 = new TreeRoot(path1);
					TreeRoot copyRoot1 = new TreeRoot(root1);
					root1.Print();
					TreeRoot root2 = new TreeRoot(path2);
					TreeRoot copyRoot2 = new TreeRoot(root2);
					root2.Print();
					root1.CompareWith(copyRoot2);
					root2.CompareWith(copyRoot1);
					root1.Print();
					root2.Print();
					ConvertToVisualTree(treeView1, root1);
					ConvertToVisualTree(treeView2, root2);
				}
			}
			catch(Exception x) {
				System.Windows.MessageBox.Show(x.Message, "Error");
			}
		}
	}
}
