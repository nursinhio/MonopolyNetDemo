using Monopoly2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Monopoly2
{
    /// <summary>
    /// Логика взаимодействия для Auction.xaml
    /// </summary>
    public partial class Auction : Window
    {
        private MainWindow mainWindow;
        private List<Player> activePlayers;
        private int currentBid;
        private int currentPlayerIndex;
        public Player highestBidder;
        public TcpClient socPlayer;


        public Auction(MainWindow mainWindow, List<Player> activePlayers)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.activePlayers = activePlayers;
            this.currentBid = 0;
            this.currentPlayerIndex = 0;
            this.highestBidder = null;
            this.socPlayer = mainWindow.socPlayer;
            UpdateCurrentPlayer();
            UpdateCurrentBid();
        }

        private void UpdateCurrentBid()
        {
            CurrentBidTextBlock.Text = currentBid.ToString();
        }

        private void UpdateCurrentPlayer()
        {
            if (activePlayers.Count > 0)
            {
                CurrentPlayer.Text = activePlayers[currentPlayerIndex].Name;
            }
        }

        private void BidButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BidTextBox.Text, out int bid))
            {
                if (bid > currentBid && bid <= activePlayers[currentPlayerIndex].Money)
                {
                    currentBid = bid;
                    highestBidder = activePlayers[currentPlayerIndex];
                    SwitchBidder();
                    UpdateCurrentBid();
                    UpdateCurrentPlayer();
                }
                else
                {
                    MessageBox.Show("Invalid bid.");
                }
            }
        }

        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            activePlayers.RemoveAt(currentPlayerIndex);
            if (activePlayers.Count == 0)
            {
                UpdateCurrentPlayer();
            }

            if (activePlayers.Count == 1)
            {
                mainWindow.IsAuctionOver = true;
                mainWindow.AuctionOffer = currentBid;
                mainWindow.AuctionWinner = highestBidder;
                mainWindow.AuctionQueue = currentPlayerIndex;
                this.Close();
            }
            else
            {
                if (currentPlayerIndex >= activePlayers.Count)
                {
                    currentPlayerIndex = 0;
                }
                UpdateCurrentPlayer();
            }
        }

        private void SwitchBidder()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % activePlayers.Count;
        }
    }
}
