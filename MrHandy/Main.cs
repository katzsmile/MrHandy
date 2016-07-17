using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace MrHandy
{
    public partial class Main : Form
    {
        public LogWindow Logger = new LogWindow();
        private const string _initVector = "tu89geji340t89u2";
        private const int _keySize = 256;
        private const int _passPhraseLength = 8;
        public JObject VaultInfo;

        public static string Encrypt(string plainText, string passPhrase)
        {
            byte[] bytes1 = Encoding.UTF8.GetBytes("tu89geji340t89u2");
            byte[] bytes2 = Encoding.UTF8.GetBytes(plainText);
            byte[] bytes3 = new Rfc2898DeriveBytes(passPhrase, bytes1).GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(bytes3, bytes1);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes2, 0, bytes2.Length);
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(array);
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            byte[] bytes1 = Encoding.ASCII.GetBytes("tu89geji340t89u2");
            byte[] buffer = Convert.FromBase64String(cipherText);
            byte[] bytes2 = new Rfc2898DeriveBytes(passPhrase, Encoding.ASCII.GetBytes("tu89geji340t89u2")).GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(bytes2, bytes1);
            MemoryStream memoryStream = new MemoryStream(buffer);
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] numArray = new byte[buffer.Length];
            int count = cryptoStream.Read(numArray, 0, numArray.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(numArray, 0, count);
        }

        public static string GeneratePassPhrase(string plainText)
        {
            string plainText1 = plainText;
            while (plainText1.Length < 8)
                plainText1 += plainText;
            return Base64Encode(plainText1).Substring(0, 8);
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string Base64Decode(string encodedText)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
        }

        public Main()
        {
            InitializeComponent();
           
        }

        public void LoadVault(string VaultData)
        {
            try
            {
                VaultInfo = JObject.Parse(VaultData);
                JToken token = VaultInfo["vault"];

                //////////////////////////////////////////////////////////////////
                string VaultName = (string)token["VaultName"];
                numVault.Value = (int)token["VaultName"];
                gbVault.Text = "Vault " + VaultName;

                //////////////////////////////////////////////////////////////////
                string VaultMode = (string)token["VaultMode"];
                switch (VaultMode)
                {
                    case "Normal": //Normal
                        cbVaultMode.Checked = false;
                        break;
                    case "Survival": //Survival
                        cbVaultMode.Checked = true;
                        break;
                    default: //Normal
                        cbVaultMode.Checked = false;
                        break;
                }

                //Lunchboxes 0 - Lunchbox, 1 - MrHandy, 2 - Pet, 3 - Starter Pack
                int LunchBoxesCount = (int)token["LunchBoxesCount"];
               
                JArray Lunchboxes = JArray.Parse(token["LunchBoxesByType"].ToString());

                lvLunchboxes.Clear();
                foreach (var o in Lunchboxes)
                {
                    switch ((int)o)
                    {
                        case 0: //Lunchbox
                            lvLunchboxes.Items.Add("Lunchbox", 0);
                            break;
                        case 1: //Mr. Handy Box
                            lvLunchboxes.Items.Add("Mr. Handy Box", 1);
                            break;
                        case 2: //Pet Container
                            lvLunchboxes.Items.Add("Pet Container", 2);
                            break;
                        case 3: //Starter Pack
                            lvLunchboxes.Items.Add("Starter Pack", 3);
                            break;
                        case 4: //Nuka Cola Quantum
                            lvLunchboxes.Items.Add("Nuka Cola Quantum", 4);
                            break;
                        default: //Lunchbox
                            lvLunchboxes.Items.Add("Lunchbox", 0);
                            break;
                    }
                    
                }
                lbLunchboxes.Text = "Lunchboxes Count: " + lvLunchboxes.Items.Count.ToString();

                if (lvLunchboxes.Items.Count > 0)
                {
                    lvLunchboxType.Enabled = true;
                }
                //////////////////////////////////////////////////////////////////
                int VaultTheme = (int)token["VaultTheme"];
                switch (VaultTheme)
                {
                    case 0: //Normal
                        cbTheme.SelectedIndex = 0;
                        break;
                    case 1: //Xmas
                        cbTheme.SelectedIndex = 1;
                        break;
                    case 2: //Halloween
                        cbTheme.SelectedIndex = 2;
                        break;
                    case 3: //ThanksGiving
                        cbTheme.SelectedIndex = 3;
                        break;
                    case 101: //BrotherOfSteel
                        cbTheme.SelectedIndex = 4;
                        break;
                    case 102: //Institute
                        cbTheme.SelectedIndex = 5;
                        break;
                    default: //Normal
                        cbTheme.SelectedIndex = 0;
                        break;
                }

                /////////////////////////////////////////////////////////////////////

                //Logger.Log("Caps: " + token["storage"]["resources"]["Nuka"].ToString());
                numCaps.Value = (int)token["storage"]["resources"]["Nuka"];
                numEnergy.Value = (int)token["storage"]["resources"]["Energy"];
                numFood.Value = (int)token["storage"]["resources"]["Food"];
                numWater.Value = (int)token["storage"]["resources"]["Water"];
                numStimpacks.Value = (int)token["storage"]["resources"]["StimPack"];
                numRadAways.Value = (int)token["storage"]["resources"]["RadAway"];
                numQuantum.Value = (int)token["storage"]["resources"]["NukaColaQuantum"];

                /////////////////////////////////////////////////////////////////////
                gbVault.Enabled = true;
                btnSave.Enabled = true;
                btnExport.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not parse Vault Data. Error: " + ex.Message);
            }
        }

        public void SaveVault(string VaultFile)
        {
            try
            {
                string VaultSave = Encrypt(VaultInfo.Root.ToString(), GeneratePassPhrase("PlayerData"));                
                File.WriteAllText(VaultFile, VaultSave);
                Logger.Log("Saved to file: " + VaultFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not parse Vault Data. Error: " + ex.Message);
            }
        }

        public void OpenSave(string VaultFile)
        {
            Logger.Log("Vault file: " + VaultFile);
            LoadVault(Decrypt(File.ReadAllText(VaultFile), GeneratePassPhrase("PlayerData")));
        }

        public void ImportJSON(string JSONFile)
        {
            Logger.Log("Importing file: " + JSONFile);
            LoadVault(File.ReadAllText(JSONFile));
        }

        private void Main_Load(object sender, EventArgs e)
        {

            this.Text += " - Version " + Application.ProductVersion;

            Logger.Show();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            Logger.Clear();
            Logger.Log("Opening Vault");

            OpenFileDialog openVault = new OpenFileDialog();

            openVault.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Fallout Shelter";
            openVault.Filter = "Fallout Shelter Save Files (*.sav)|*.sav|All files (*.*)|*.*";
            openVault.FilterIndex = 1;
            openVault.RestoreDirectory = true;

            if (openVault.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openVault.FileName != null)
                    {                        
                        OpenSave(openVault.FileName);              
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Logger.Clear();
            Logger.Log("Saving Vault");

            SaveFileDialog saveVaultFile = new SaveFileDialog();

            saveVaultFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Fallout Shelter";
            saveVaultFile.Filter = "Fallout Shelter Save Files (*.sav)|*.sav|All files (*.*)|*.*";
            saveVaultFile.FilterIndex = 1;
            saveVaultFile.RestoreDirectory = true;

            if (saveVaultFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SaveVault(saveVaultFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
                }
            }
        }

        private void numVault_ValueChanged(object sender, EventArgs e)
        {
            string VaultName = numVault.Value.ToString();
            VaultInfo["vault"]["VaultName"] = VaultName;
            Logger.Log(VaultName);
            gbVault.Text = "Vault " + VaultName;
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (char)Keys.None;
        }

        private void numLunchBoxes_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Logger.Clear();
            Logger.Log("Exporting to JSON");

            SaveFileDialog saveVaultFile = new SaveFileDialog();

            saveVaultFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Fallout Shelter";
            saveVaultFile.Filter = "JSON File (*.json)|*.json|All files (*.*)|*.*";
            saveVaultFile.FilterIndex = 1;
            saveVaultFile.RestoreDirectory = true;

            if (saveVaultFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Logger.Log("Exported to file: " + saveVaultFile.FileName);
                    string VaultSave = VaultInfo.Root.ToString();
                    File.WriteAllText(saveVaultFile.FileName, VaultSave);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Logger.Clear();
            Logger.Log("Importing JSON");

            OpenFileDialog openVault = new OpenFileDialog();

            openVault.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Fallout Shelter";
            openVault.Filter = "JSON File (*.json)|*.json|All files (*.*)|*.*";
            openVault.FilterIndex = 1;
            openVault.RestoreDirectory = true;

            if (openVault.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openVault.FileName != null)
                    {
                        ImportJSON(openVault.FileName);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void numVault_ValueChanged(object sender, KeyEventArgs e)
        {

        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            numCaps.Value = 9999999;
            numEnergy.Value = 9999999;
            numFood.Value = 9999999;
            numWater.Value = 9999999;
            numStimpacks.Value = 9999999;
            numRadAways.Value = 9999999;
            numQuantum.Value = 9999999;
        }

        private void btnZero_Click(object sender, EventArgs e)
        {
            numCaps.Value = 0;
            numEnergy.Value = 0;
            numFood.Value = 0;
            numWater.Value = 0;
            numStimpacks.Value = 0;
            numRadAways.Value = 0;
            numQuantum.Value = 0;
        }

        private void numCaps_ValueChanged(object sender, EventArgs e)
        {
            int Caps = (int)numCaps.Value;
            VaultInfo["vault"]["storage"]["resources"]["Nuka"] = Caps;
            Logger.Log("Caps: " + Caps.ToString());
        }

        private void numEnergy_ValueChanged(object sender, EventArgs e)
        {
            int Energy = (int)numEnergy.Value;
            VaultInfo["vault"]["storage"]["resources"]["Energy"] = Energy;
            Logger.Log("Energy: " + Energy.ToString());
        }

        private void numFood_ValueChanged(object sender, EventArgs e)
        {
            int Food = (int)numFood.Value;
            VaultInfo["vault"]["storage"]["resources"]["Food"] = Food;
            Logger.Log("Food: " + Food.ToString());
        }

        private void numWater_ValueChanged(object sender, EventArgs e)
        {
            int Water = (int)numWater.Value;
            VaultInfo["vault"]["storage"]["resources"]["Water"] = Water;
            Logger.Log("Water: " + Water.ToString());
        }

        private void numStimpacks_ValueChanged(object sender, EventArgs e)
        {
            int Stimpack = (int)numStimpacks.Value;
            VaultInfo["vault"]["storage"]["resources"]["Stimpack"] = Stimpack;
            Logger.Log("Stimpack: " + Stimpack.ToString());
        }

        private void numRadAways_ValueChanged(object sender, EventArgs e)
        {
            int RadAway = (int)numRadAways.Value;
            VaultInfo["vault"]["storage"]["resources"]["RadAway"] = RadAway;
            Logger.Log("RadAway: " + RadAway.ToString());
        }

        private void numQuantum_ValueChanged(object sender, EventArgs e)
        {
            int NukaColaQuantum = (int)numQuantum.Value;
            VaultInfo["vault"]["storage"]["resources"]["NukaColaQuantum"] = NukaColaQuantum;
            Logger.Log("NukaColaQuantum: " + NukaColaQuantum.ToString());
        }

        private void cbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            int VaultTheme = 0;

            switch (cbTheme.SelectedIndex)
            {
                case 0: //Normal
                    VaultTheme = 0;
                    break;
                case 1: //Xmas
                    VaultTheme = 1;
                    break;
                case 2: //Halloween
                    VaultTheme = 2;
                    break;
                case 3: //ThanksGiving
                    VaultTheme = 3;
                    break;
                case 4: //BrotherOfSteel
                    VaultTheme = 101;
                    break;
                case 5: //Institute
                    VaultTheme = 102;
                    break;
                default: //Normal
                    VaultTheme = 0;
                    break;
            }
            VaultInfo["vault"]["VaultTheme"] = VaultTheme;
            Logger.Log("VaultTheme: " + VaultTheme.ToString());
        }

        private void cbVaultMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbVaultMode.Checked)
            {
                VaultInfo["vault"]["VaultMode"] = "Normal";
                Logger.Log("VaultMode: " + "Normal");
            }
            else
            {
                VaultInfo["vault"]["VaultMode"] = "Survival";
                Logger.Log("VaultMode: " + "Survival");
            }
        }

        public void ReadLunchboxInfo(int LunchboxType)
        {
            
            switch (LunchboxType)
            {
                case 0: //Lunchbox
                    txtLunchbox.Text = "Description:\nLunchboxes contain five cards, one of which is guaranteed to be at least rare or better.\nCards include weapons, dwellers, resources, caps or junk items.";
                    lvLunchboxes.SelectedItems[0].ImageIndex = 0;
                    lvLunchboxes.SelectedItems[0].Text = "Lunchbox";
                    break;
                case 1: //Mr. Handy Box
                    txtLunchbox.Text = "Description:\nMr. Handy Box is giving you Mr. Handy robot as dweller.";
                    lvLunchboxes.SelectedItems[0].ImageIndex = 1;
                    lvLunchboxes.SelectedItems[0].Text = "Mr. Handy Box";
                    break;
                case 2: //Pet Container
                    txtLunchbox.Text = "Description:\nPet Container is giving you one random animal pet as new pet for your vault.";
                    lvLunchboxes.SelectedItems[0].ImageIndex = 2;
                    lvLunchboxes.SelectedItems[0].Text = "Pet Container";
                    break;
                case 3: //Starter Pack
                    txtLunchbox.Text = "Description:\nStarter Pack contain set of twelve cards, one of which is guaranteed to be at least rare or better, and one will have 3 Nuka Cola Quantum bottles.\nCards include weapons, dwellers, resources, caps or junk items.";
                    lvLunchboxes.SelectedItems[0].ImageIndex = 3;
                    lvLunchboxes.SelectedItems[0].Text = "Starter Pack";
                    break;
                case 4: //Nuka Cola Quantum
                    txtLunchbox.Text = "Description:\nNuka Cola Quantum Lunchbox contain one card with random quantity of Nuka Cola Quantum bottles.";
                    lvLunchboxes.SelectedItems[0].ImageIndex = 4;
                    lvLunchboxes.SelectedItems[0].Text = "Nuka Cola Quantum";
                    break;
                default: //Lunchbox
                    txtLunchbox.Text = "Description:\nLunchboxes contain five cards, one of which is guaranteed to be at least rare or better.\nCards include weapons, dwellers, resources, caps or junk items.";
                    lvLunchboxes.SelectedItems[0].ImageIndex = 0;
                    lvLunchboxes.SelectedItems[0].Text = "Lunchbox";
                    break;
            }
        }

        private void lvLunchboxes_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var selectedItems = lvLunchboxes.SelectedItems;
            if (selectedItems.Count > 0)
            {
                // Display text of first item selected.  
                lvLunchboxType.SelectedIndices.Clear();
                lvLunchboxType.Items[selectedItems[0].ImageIndex].Focused = true;
                lvLunchboxType.Items[selectedItems[0].ImageIndex].Selected = true;
                ReadLunchboxInfo(selectedItems[0].ImageIndex);
            }
        }

        private void lvLunchboxType_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var selectedItems = lvLunchboxes.SelectedItems;
            if (selectedItems.Count > 0)
            {
                var TypeselectedItems = lvLunchboxType.SelectedItems;
                if (TypeselectedItems.Count > 0)
                {
                    // Display text of first item selected.                  
                    ReadLunchboxInfo(TypeselectedItems[0].ImageIndex);

                    string LunchBoxesByType = "[\n";
                    foreach (ListViewItem o in lvLunchboxes.Items)
                    {
                        LunchBoxesByType += o.ImageIndex.ToString() + ",\n";
                    }
                    LunchBoxesByType += "]";
                    JArray Lunchboxes = JArray.Parse(LunchBoxesByType);
                    VaultInfo["vault"]["LunchBoxesByType"] = Lunchboxes;
                    VaultInfo["vault"]["LunchBoxesCount"] = lvLunchboxes.Items.Count;
                    lbLunchboxes.Text = "Lunchboxes Count: " + lvLunchboxes.Items.Count.ToString();

                    Logger.Log("Lunchbox: " + VaultInfo["vault"]["LunchBoxesByType"].ToString());
                }
            }
        }

        private void btnAddLunchbox_Click(object sender, EventArgs e)
        {
            lvLunchboxes.Items.Add("Lunchbox", 0);

            var r = Enumerable.Empty<ListViewItem>();

            if (this.lvLunchboxes.Items.Count > 0)
                r = this.lvLunchboxes.Items.OfType<ListViewItem>();

            var last = r.LastOrDefault();

            if (last != null)
            {
                lvLunchboxes.SelectedIndices.Clear();
                lvLunchboxes.Items[last.Index].Focused = true;
                lvLunchboxes.Items[last.Index].Selected = true;
                lvLunchboxType.Enabled = true;
            }
            
        }

        private void btnRemoveLunchbox_Click(object sender, EventArgs e)
        {
            var selectedItems = lvLunchboxes.SelectedItems;
            if (selectedItems.Count > 0)
            {
                lvLunchboxes.SelectedItems[0].Remove();
                lvLunchboxes.SelectedIndices.Clear();

                string LunchBoxesByType = "[]";
                JArray Lunchboxes = JArray.Parse(LunchBoxesByType);
                VaultInfo["vault"]["LunchBoxesByType"] = Lunchboxes;
                VaultInfo["vault"]["LunchBoxesCount"] = lvLunchboxes.Items.Count;
                lbLunchboxes.Text = "Lunchboxes Count: " + lvLunchboxes.Items.Count.ToString();

                Logger.Log("Lunchbox: " + VaultInfo["vault"]["LunchBoxesByType"].ToString());

                if (lvLunchboxes.Items.Count > 0)
                {
                    var r = Enumerable.Empty<ListViewItem>();

                    if (this.lvLunchboxes.Items.Count > 0)
                        r = this.lvLunchboxes.Items.OfType<ListViewItem>();

                    var last = r.LastOrDefault();

                    if (last != null)
                    {
                        lvLunchboxes.Items[last.Index].Focused = true;
                        lvLunchboxes.Items[last.Index].Selected = true;
                        lvLunchboxType.Enabled = true;
                    }
                }
                else
                {
                    lvLunchboxType.Enabled = false;
                }

            }
        }
    }
}
