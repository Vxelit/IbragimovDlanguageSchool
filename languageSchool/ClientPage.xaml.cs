﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace languageSchool
{
	/// <summary>
	/// Логика взаимодействия для ClientPage.xaml
	/// </summary>
	public partial class ClientPage : Page
	{

		int CountRecords;
		int CountPage;
		int CurrentPage = 0;
		int recordsAmountOnList = 10;

		List<Client> CurrentPageList = new List<Client>();
		List<Client> TableList;

		public ClientPage()
		{
			InitializeComponent();

			var clientsItemList = IbragimovDLanguageEntities.GetContext().Client.ToList();


			ComboCountRecords.SelectedIndex = 0;
			// define recordsAmountOnList 

			if (ComboCountRecords.SelectedIndex == 0)
				recordsAmountOnList = 10;
			else if (ComboCountRecords.SelectedIndex == 1)
			{
				recordsAmountOnList = 50;
			}
			else if (ComboCountRecords.SelectedIndex == 2)
			{
				recordsAmountOnList = 200;
			}
			else if (ComboCountRecords.SelectedIndex == 3)
			{
				recordsAmountOnList = clientsItemList.Count;
			}



			UpdateClients();
		}

		private void UpdateClients()
		{
			var currentClients = IbragimovDLanguageEntities.GetContext().Client.ToList();

			
			
			ClientListView.ItemsSource = currentClients.ToList();
			
			
			//
			
			TableList = currentClients;
            ChangePage(0, 0);

        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
		{
			ChangePage(2, null);
		}

		private void LeftDirButton_Click(object sender, RoutedEventArgs e)
		{
            ChangePage(1, null);

        }

        private void ChangePage(int direction, int? selectedPage)
		{
			CurrentPageList.Clear();
			CountRecords = TableList.Count;

			if (CountRecords % recordsAmountOnList > 0)
			{
				CountPage = CountRecords / recordsAmountOnList + 1;
			} else
			{
				CountPage = CountRecords / recordsAmountOnList;
			}

			Boolean Ifupdate = true;

			int min;

			if (selectedPage.HasValue)
			{
				if (selectedPage >= 0 && selectedPage <= CountRecords)
				{
					CurrentPage = (int)selectedPage;
					min = CurrentPage * recordsAmountOnList + recordsAmountOnList < CountRecords ? CurrentPage * recordsAmountOnList + recordsAmountOnList : CountRecords;
					for (int i = CurrentPage * recordsAmountOnList; i < min; i++)
					{
						CurrentPageList.Add(TableList[i]);
					}
				}
			}
			else
			{
				switch(direction)
				{
					case 1: 
						if (CurrentPage > 0)
						{
							CurrentPage--;
							min = CurrentPage * recordsAmountOnList + recordsAmountOnList < CountPage ? CurrentPage * recordsAmountOnList + recordsAmountOnList : CountRecords;
							for (int i = CurrentPage * recordsAmountOnList; i < min; i++)
							{
								CurrentPageList.Add(TableList[i]);
							}
						}
						else
						{
							Ifupdate = false;
						}
						break;

					case 2:
						if (CurrentPage < CountPage - 1)
						{
							CurrentPage++;
							min = CurrentPage * recordsAmountOnList + recordsAmountOnList < CountRecords ? CurrentPage * recordsAmountOnList + recordsAmountOnList : CountRecords;
							for (int i = CurrentPage * recordsAmountOnList; i < min; i++)
							{
								CurrentPageList.Add(TableList[i]);
							}
						}
						else
						{
							Ifupdate = false;
						}
						break;
				}
			}

            if (Ifupdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;
                min = CurrentPage * recordsAmountOnList + recordsAmountOnList < CountRecords ? CurrentPage * recordsAmountOnList + recordsAmountOnList : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();

                ClientListView.ItemsSource = CurrentPageList;
                ClientListView.Items.Refresh();
            }
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
			ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void ComboCountRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboCountRecords.SelectedIndex == 0)
                recordsAmountOnList = 10;
            else if (ComboCountRecords.SelectedIndex == 1)
            {
                recordsAmountOnList = 50;
            }
            else if (ComboCountRecords.SelectedIndex == 2)
            {
                recordsAmountOnList = 200;
            }
            else if (ComboCountRecords.SelectedIndex == 3)
            {
                recordsAmountOnList = IbragimovDLanguageEntities.GetContext().Client.ToList().Count;
            }


            UpdateClients();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
			var currentClient = (sender as Button).DataContext as Client;

			var currentClientServices = IbragimovDLanguageEntities.GetContext().ClientService.ToList();
			currentClientServices = currentClientServices.Where(p => p.ClientID == currentClient.ID).ToList();

			if (currentClientServices.Count != 0)
			{
				MessageBox.Show("Невозможно выполнить удаление, так как у клиента было посещение");
			} else
			{
				if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					try
					{
						IbragimovDLanguageEntities.GetContext().Client.Remove(currentClient);
						IbragimovDLanguageEntities.GetContext().SaveChanges();

						UpdateClients();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message.ToString());
					}
				}
			}
        }
    }
}
