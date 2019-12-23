﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace alacakVerecekTakip
{
    public partial class customerDebtDetail : MetroFramework.Forms.MetroForm
    {
        public customerDebtDetail()
        {
            InitializeComponent();
        }

        methods funcs = new methods();
        SqlConnection baglanti = methods.baglanti;
        string theme;

        private void fillCustomerTransactionListViewColumns()
        {
            customerTransactionListView.Items.Clear();
            customerTransactionListView.View = View.Details;
            customerTransactionListView.GridLines = true;
            customerTransactionListView.Columns.Add("Müşteri Adı Soyadı", 122);
            customerTransactionListView.Columns.Add("İşlem Tipi", 122);
            customerTransactionListView.Columns.Add("İşlem Tutarı", 122);
            customerTransactionListView.Columns.Add("Para Türü", 122);
            customerTransactionListView.Columns.Add("İşlem Tarihi", 122);
            customerTransactionListView.Columns.Add("İşlem Id", 122);
        }

        private void fillCustomersInfo(int customerId)
        {
            string[] reliabilityTable = findReliabilityTable();
            SqlCommand fillCustomersInfoCommand = new SqlCommand("SELECT * FROM customers WHERE customerId = @customerId", baglanti);
            fillCustomersInfoCommand.Parameters.AddWithValue("@customerId", customerId);
            SqlDataReader sdr = fillCustomersInfoCommand.ExecuteReader();
            while (sdr.Read())
            {
                customerIdText.Text = sdr["customerId"].ToString();
                customerNameText.Text = sdr["customerName"].ToString();
                customerSurnameText.Text = sdr["customerSurname"].ToString();
                customerPhoneText.Text = sdr["customerPhone"].ToString();
                customerMailText.Text = sdr["customerMail"].ToString();
                customerAdressRichText.Text = sdr["customerAdress"].ToString();
                for (int i = 0; i < reliabilityTable.Length; i++)
                {
                    string[] reliabilityTableDetail = reliabilityTable[i].Split('-');
                    if (Convert.ToInt32(reliabilityTableDetail[0]) == Convert.ToInt32(sdr["customerReliabilityVal"]))
                    {
                        customerReliabiltyText.Text = reliabilityTableDetail[1];
                    }
                }
            }
            sdr.Close();
        }

        private void fillCustomerTransactionListViewItems(int customerId)
        {
            int rowCount = 0;
            string[] customersDebtorTable = findCustomersDebtorTable(), customersMyDebtTable = findCustomersMyDebtTable(), moneyTypesTable = findExchangeMoneyFromIdToName();
            SqlCommand fillCustomerTransactionListViewCommand = new SqlCommand("SELECT * FROM customersTransactionType WHERE customerId = @customerId", baglanti);
            fillCustomerTransactionListViewCommand.Parameters.AddWithValue("@customerId", customerId);
            SqlDataReader sdr = fillCustomerTransactionListViewCommand.ExecuteReader();
            ListViewItem li = new ListViewItem();
            while (sdr.Read())
            {
                li = customerTransactionListView.Items.Add(customerNameText.Text + " " + customerSurnameText.Text);
                if (Convert.ToInt32(sdr["transactionType"]) == 0)li.SubItems.Add("Borç Alma");
                else li.SubItems.Add("Borç Verme");

                if (Convert.ToInt32(sdr["transactionType"]) == 0){
                    for (int i = 0; i < customersMyDebtTable.Length; i++){
                        string[] customersMyDebtTableDetail = customersMyDebtTable[i].Split('-');
                        if (Convert.ToInt32(customersMyDebtTableDetail[2]) == Convert.ToInt32(sdr["customerTransactionTypeId"])){
                            li.SubItems.Add(customersMyDebtTableDetail[4]);
                            for (int k = 0; k < moneyTypesTable.Length; k++){
                                string[] moneyTypesTableDetail = moneyTypesTable[k].Split('-');
                                if (Convert.ToInt32(customersMyDebtTableDetail[5]) == Convert.ToInt32(moneyTypesTableDetail[0])){
                                    li.SubItems.Add(moneyTypesTableDetail[1]);
                                }

                            }
                            break;
                        }
                    }
                }
                else if (Convert.ToInt32(sdr["transactionType"]) == 1)
                {
                    for (int j = 0; j < customersDebtorTable.Length; j++){
                        string[] customersDebtorTableDetail = customersDebtorTable[j].Split('-');
                        if (Convert.ToInt32(customersDebtorTableDetail[2]) == Convert.ToInt32(sdr["customerTransactionTypeId"])){
                            li.SubItems.Add(customersDebtorTableDetail[4]);
                            for (int k = 0; k < moneyTypesTable.Length; k++){
                                string[] moneyTypesTableDetail = moneyTypesTable[k].Split('-');
                                if (Convert.ToInt32(customersDebtorTableDetail[5]) == Convert.ToInt32(moneyTypesTableDetail[0])){
                                    li.SubItems.Add(moneyTypesTableDetail[1]);
                                }

                            }
                            break;
                        }
                    }
                }
                li.SubItems.Add(sdr["transactionDate"].ToString());
                li.SubItems.Add(sdr["customerTransactionTypeId"].ToString());
                rowCount++;
            }
            sdr.Close();
            customerTransactionListViewLabel.Text =  "'" + customerNameText.Text + " " + customerSurnameText.Text + "' adlı müşteriye ait " + rowCount.ToString() + " adet kayıt bulundu.";
        }

        private string[] findReliabilityTable()
        {
            int reliabilityCount = reliabiltyTableRowsCount(), reliabilityCount2 = 0;
            string[] reliabiltyTable = new string[reliabilityCount];

            SqlCommand findCustomerDebtValTableCommand = new SqlCommand("SELECT * FROM degreeOfReliabilty", baglanti);
            SqlDataReader sdr = findCustomerDebtValTableCommand.ExecuteReader();
            while (sdr.Read())
            {
                if (reliabilityCount2 <= reliabilityCount)
                {
                    reliabiltyTable[reliabilityCount2] = (sdr["degreeOfRealiabiltyId"].ToString()) + "-" + sdr["degreeOfReliabiltyDiscription"].ToString();
                    reliabilityCount2++;
                }
            }
            sdr.Close();
            return reliabiltyTable;
        }

        private int reliabiltyTableRowsCount()
        {
            int rowCount = 0;
            SqlCommand reliabiltyTableRowsCountCommand = new SqlCommand("SELECT * FROM degreeOfReliabilty", baglanti);
            SqlDataReader sdr = reliabiltyTableRowsCountCommand.ExecuteReader();
            while (sdr.Read())
            {
                rowCount++;
            }
            sdr.Close();
            return rowCount;
        }

        private string[] findCustomersDebtorTable()
        {
            int customersTableRowCount = customersDebtorTableRowCount(), customersTableRowCount2 = 0;
            string[] customersDebtorTable = new string[customersTableRowCount];

            SqlCommand findCustomerDebtValTableCommand = new SqlCommand("SELECT * FROM customersDebtor", baglanti);
            SqlDataReader sdr = findCustomerDebtValTableCommand.ExecuteReader();
            while (sdr.Read())
            {
                if (customersTableRowCount2 <= customersTableRowCount)
                {
                    customersDebtorTable[customersTableRowCount2] = sdr["debtorId"].ToString() + "-" + sdr["customerId"].ToString() + "-" + sdr["transactionTypeId"].ToString() + "-" + sdr["debtType"].ToString() + "-" + sdr["debtVal"].ToString() + "-" + sdr["debtMoneyTypeId"].ToString() + "-" + sdr["debtBankTypeId"].ToString() + "-" + sdr["debtDate"].ToString() + "-" + sdr["debtPaymentDate"].ToString();
                    customersTableRowCount2++;
                }
            }
            sdr.Close();
            return customersDebtorTable;
        }



        private int customersDebtorTableRowCount()
        {
            int rowCount = 0;
            SqlCommand rowCountCommand = new SqlCommand("SELECT * FROM customersDebtor", baglanti);
            SqlDataReader sdr = rowCountCommand.ExecuteReader();
            while (sdr.Read())
            {
                rowCount++;
            }
            sdr.Close();
            return rowCount;
        }

        private string[] findCustomersMyDebtTable()
        {
            int customersTableRowCount = customersMyDebtTableRowCount(), customersTableRowCount2 = 0;
            string[] customersDebtorTable = new string[customersTableRowCount];

            SqlCommand findCustomerDebtValTableCommand = new SqlCommand("SELECT * FROM customersMyDebt", baglanti);
            SqlDataReader sdr = findCustomerDebtValTableCommand.ExecuteReader();
            while (sdr.Read())
            {
                if (customersTableRowCount2 <= customersTableRowCount)
                {
                    customersDebtorTable[customersTableRowCount2] = sdr["myDebtId"].ToString() + "-" + sdr["customerId"].ToString() + "-" + sdr["transactionTypeId"].ToString() + "-" + sdr["debtType"].ToString() + "-" + sdr["debtVal"].ToString() + "-" + sdr["debtMoneyTypeId"].ToString() + "-" + sdr["debtBankTypeId"].ToString() + "-" + sdr["debtDate"].ToString() + "-" + sdr["debtPaymentDate"].ToString();
                    customersTableRowCount2++;
                }
            }
            sdr.Close();
            return customersDebtorTable;
        }



        private int customersMyDebtTableRowCount()
        {
            int rowCount = 0;
            SqlCommand rowCountCommand = new SqlCommand("SELECT * FROM customersMyDebt", baglanti);
            SqlDataReader sdr = rowCountCommand.ExecuteReader();
            while (sdr.Read())
            {
                rowCount++;
            }
            sdr.Close();
            return rowCount;
        }

        private string[] findExchangeMoneyFromIdToName()
        {
            int moneyCount1 = moneyCount(), moneyCount2 = 0;
            string[] exchangeMoneyName = new string[moneyCount1];

            SqlCommand findExchangeMoneyFromIdToNameCommand = new SqlCommand("SELECT * FROM moneyTypesTable", baglanti);
            SqlDataReader sdr = findExchangeMoneyFromIdToNameCommand.ExecuteReader();
            while (sdr.Read())
            {
                if (moneyCount2 <= moneyCount1)
                {
                    exchangeMoneyName[moneyCount2] = (sdr["moneyId"].ToString()) + "-" + sdr["moneyName"].ToString();
                    moneyCount2++;
                }
            }
            sdr.Close();
            return exchangeMoneyName;
        }

        private int moneyCount()
        {
            int rowCount = 0;
            SqlCommand rowCountCommand = new SqlCommand("SELECT * FROM moneyTypesTable", baglanti);
            SqlDataReader sdr = rowCountCommand.ExecuteReader();
            while (sdr.Read())
            {
                rowCount++;
            }
            sdr.Close();
            return rowCount;
        }

        private void customerDebtDetail_Load(object sender, EventArgs e)
        {
            this.StyleManager = metroStyleManager1;
            theme = funcs.themeChanger(0);

            if (theme == "light") metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
            else metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
            if (funcs.isConnect(baglanti) == true) { }
            else
            {
                MetroFramework.MetroMessageBox.Show(this, "Veri Tabanı Bağlantısı Kurulamadığından Dolayı Program Kapatılıyor..", "BİLGİ!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            if (funcs.isConnect(baglanti) == true)
            {
                connectSituation.BackColor = Color.Lime;
                funcs.setToolTip(connectSituation, "Veri Tabanı Bağlantısı Var");
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this, "Veri Tabanı Bağlantısı Kurulamadığından Dolayı Program Kapatılıyor..", "BİLGİ!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            if (metroStyleManager1.Theme == MetroFramework.MetroThemeStyle.Dark)
            {
                helpPictureBox.Image = alacakVerecekTakip.Properties.Resources.helpWhite;
                customerPhoneText.BackColor = Color.FromArgb(17, 17, 17);
                customerPhoneText.ForeColor = Color.Silver;
                customerAdressRichText.BackColor = Color.FromArgb(17, 17, 17);
                customerAdressRichText.ForeColor = Color.Silver;
            }
            else
            {
                helpPictureBox.Image = alacakVerecekTakip.Properties.Resources.help;
                customerPhoneText.BackColor = Color.White;
                customerPhoneText.ForeColor = Color.Black;
                customerAdressRichText.BackColor = Color.White;
                customerAdressRichText.ForeColor = Color.Black;
            }


            fillCustomersInfo(showAllCustomers.selectedCustomerId);
            fillCustomerTransactionListViewColumns();
            fillCustomerTransactionListViewItems(showAllCustomers.selectedCustomerId);
        }
    }
}