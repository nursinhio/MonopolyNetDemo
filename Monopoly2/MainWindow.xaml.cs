using Monopoly2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
using System.Xaml;
using static System.Collections.Specialized.BitVector32;
namespace Monopoly2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, Coordinates> cellCenters;
        public List<Player> players;
        List<TextBlock> textBlocks;

        public int currentPlayerIndex;
        public Dictionary<int, Property> properties;
        public int Queue = 1;
        public Player AuctionWinner { get; set; }

        //Auction
        public int AuctionOffer;
        public bool isAuctionDeclined;
        public int AuctionQueue;
        public bool IsAuctionOver;


        //Jail
        public bool IsPlayerInJail = false;
        //Thread
        public Mutex Mutex = new Mutex();

        public MainWindow()
        {

            InitializeComponent();
            players = new List<Player>
            {
                new Player(),
                new Player("Blue", Brushes.Blue)
            };
            textBlocks = new List<TextBlock>
            {
                Red, Blue
            };
            InitializeCellCentersAndProperties();
            foreach (Player player in players)
            {
                DrawPlayer(player);
            }
        }

        private void SwitchToNextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }
        public void DrawPlayer(Player player)
        {
            Canvas.SetLeft(player.ellipse, cellCenters[player.Position].X);
            Canvas.SetTop(player.ellipse, cellCenters[player.Position].Y);
            GameBoardCanvas.Children.Add(player.ellipse);
        }



        private void InitializeCellCentersAndProperties()
        {
            cellCenters = new Dictionary<int, Coordinates>
            {
                { 0, new Coordinates(50 + 30, 50 + 30) }, // Go
                { 1, new Coordinates(110 + 30, 50 + 30) }, // Aqsay
                { 2, new Coordinates(170 + 30, 50 + 30) }, // Qazyna
                { 3, new Coordinates(230 + 30, 50 + 30) }, // Zhetisu
                { 4, new Coordinates(290 + 30, 50 + 30) }, // Salyq
                { 5, new Coordinates(350 + 30, 50 + 30) }, // Temir Zhol 1
                { 6, new Coordinates(410 + 30, 50 + 30) }, // Suinbay
                { 7, new Coordinates(470 + 30, 50 + 30) }, // Mumkinshilik
                { 8, new Coordinates(530 + 30, 50 + 30) }, // Rysqulov
                { 9, new Coordinates(590 + 30, 50 + 30) }, // Raimbek
                { 10, new Coordinates(650 + 30, 50 + 30) }, // Jail
                { 11, new Coordinates(650 + 30, 110 + 30) }, // Qabanbay
                { 12, new Coordinates(650 + 30, 170 + 30) }, // Elektrostansa
                { 13, new Coordinates(650 + 30, 230 + 30) }, // Nauryzbay
                { 14, new Coordinates(650 + 30, 290 + 30) }, // Bogenbai
                { 15, new Coordinates(650 + 30, 350 + 30) }, // Temir Zhol 2
                { 16, new Coordinates(650 + 30, 410 + 30) }, // Aiteke Bi
                { 17, new Coordinates(650 + 30, 470 + 30) }, // Qazyna
                { 18, new Coordinates(650 + 30, 530 + 30) }, // Qazybel Bi
                { 19, new Coordinates(650 + 30, 590 + 30) }, // Tole Bi
                { 20, new Coordinates(650 + 30, 650 + 30) }, // Free Parking
                { 21, new Coordinates(590 + 30, 650 + 30) }, // Auezov
                { 22, new Coordinates(530 + 30, 650 + 30) }, // Mumkinshilik
                { 23, new Coordinates(470 + 30, 650 + 30) }, // Seifullin
                { 24, new Coordinates(410 + 30, 650 + 30) }, // Abay
                { 25, new Coordinates(350 + 30, 650 + 30) }, // Temir Zhol 3
                { 26, new Coordinates(290 + 30, 650 + 30) }, // Zheltoqsan
                { 27, new Coordinates(230 + 30, 650 + 30) }, // Dostyq
                { 28, new Coordinates(170 + 30, 650 + 30) }, // Su Qabyry
                { 29, new Coordinates(110 + 30, 650 + 30) }, // Beibitshilik
                { 30, new Coordinates(50 + 30, 650 + 30) }, // Go to Jail
                { 31, new Coordinates(50 + 30, 590 + 30) }, // Satpaev
                { 32, new Coordinates(50 + 30, 530 + 30) }, // Baitursynov
                { 33, new Coordinates(50 + 30, 470 + 30) }, // Qazyna
                { 34, new Coordinates(50 + 30, 410 + 30) }, // Abylai Khan
                { 35, new Coordinates(50 + 30, 350 + 30) }, // Temir Zhol 4
                { 36, new Coordinates(50 + 30, 290 + 30) }, // Mumkinshilik
                { 37, new Coordinates(50 + 30, 230 + 30) }, // Astana
                { 38, new Coordinates(50 + 30, 170 + 30) }, // Salyq
                { 39, new Coordinates(50 + 30, 110 + 30) } // Respublika
            };

            properties = new Dictionary<int, Property>
            {
                { 0, new Property(Go, 0, 0) },
                { 1, new Property(Aqsay, 60, 15) },
                { 2, new Property(Qazyna, 0, 15) },
                { 3, new Property(Zhetisu, 60, 10) },
                { 4, new Property(Salyq, 0, 0) },
                { 5, new Property(TemirZhol1, 200, 50) },
                { 6, new Property(Suinbay, 100, 25) },
                { 7, new Property(Mumkinshilik, 0, 0) },
                { 8, new Property(Rysqulov, 100, 25) },
                { 9, new Property(Raimbek, 120, 30) },
                { 10, new Property(Jail, 0, 0) },
                { 11, new Property(Qabanbay, 140, 35) },
                { 12, new Property(Elektrostansa, 150, 37) },
                { 13, new Property(Nauryzbay, 140, 37) },
                { 14, new Property(Bogenbai, 160, 40) },
                { 15, new Property(TemirZhol2, 200, 50) },
                { 16, new Property(AitekeBi, 180, 45) },
                { 17, new Property(Qazyna, 0, 0) },
                { 18, new Property(QazybekBi, 180, 45) },
                { 19, new Property(ToleBi, 150, 37) },
                { 20, new Property(FreeParkinh, 0, 0) },
                { 21, new Property(Auezov, 220, 55) },
                { 22, new Property(Mumkinshilik1, 0, 0) },
                { 23, new Property(Seifullin, 220, 55) },
                { 24, new Property(Abay, 240, 60) },
                { 25, new Property(TemirZhol3, 200, 50) },
                { 26, new Property(Zheltoqsan, 260, 65) },
                { 27, new Property(Dostyq, 260, 65) },
                { 28, new Property(SuQubyry, 150, 35) },
                { 29, new Property(Beibitshilik, 280, 70) },
                { 30, new Property(GoToJail, 0, 0) },
                { 31, new Property(Satpaev, 300, 75) },
                { 32, new Property(Baitursynov, 300, 75) },
                { 33, new Property(Qazyna, 0, 0) },
                { 34, new Property(AbylaiKhan, 320, 80) },
                { 35, new Property(TemirZhol4, 200, 50) },
                { 36, new Property(Mumkinshilik2, 0, 0) },
                { 37, new Property(Astana, 350, 87) },
                { 38, new Property(Salyq1, 0, 0) },
                { 39, new Property(Respublika, 400, 100) }
            };

        }

        //private async void MovePlayer(Player player, int steps)
        //{
        //    for (int i = 0; i < steps; i++)
        //    {
        //        player.Position = (player.Position + 1) % 40;
        //        if (IsPassedGo())
        //        {
        //            player.Money += 1000;
        //        }
        //        await AnimatePlayerMove(player);
        //        await Task.Delay(200);
        //    }

        //    if (properties.ContainsKey(player.Position) && properties[player.Position].Owner == null)
        //    {
        //        if (player.Money < properties[player.Position].Price)
        //        {
        //            StartAuction(player);

        //        }
        //        else if (IsPlayerAtGoToJail(player))
        //        {
        //            ToJail(player);

        //        }
        //        else
        //        {
        //            if (CantBuyProperty())
        //            {

        //                if (IsQazyna())
        //                {
        //                    Random rnd = new Random(
        //                    (int)(Thread.CurrentThread.ManagedThreadId * DateTime.Now.Ticks));
        //                    int earn = Math.Abs(rnd.Next()) % 500 + 1;
        //                    player.Money += earn;
        //                    System.Windows.MessageBox.Show("You earn " + earn.ToString());
        //                }
        //                else if (IsMumkinshilik())
        //                {
        //                    System.Windows.MessageBox.Show("Mumkinshilik");
        //                }
        //                else if (IsSalyq())
        //                {
        //                    player.Money -= 300;
        //                    System.Windows.MessageBox.Show("You paid " + 300.ToString() + "to bank", "Salyq", MessageBoxButton.OK);
        //                }
        //            }
        //            else
        //            {
        //                var result = System.Windows.MessageBox.Show("Purchase?", "Buy", MessageBoxButton.YesNo);
        //                if (result == MessageBoxResult.Yes)
        //                {
        //                    player.Money -= properties[player.Position].Price;
        //                    properties[player.Position].Rectangle.Fill = player.playerColor;
        //                    properties[player.Position].Owner = player;
        //                    //player.proprties.Add(properties[player.Position]);
        //                }
        //                else
        //                {
        //                    StartAuction(player);
        //                }
        //            }
        //        }
        //        if (player.IsPlayerInJail == true)
        //        {
        //            player.SessionsInJail++;

        //        }
        //        if (player.SessionsInJail == 2)
        //        {
        //            player.IsPlayerInJail = false;
        //            player.SessionsInJail = 0;

        //        }
        //    }//At players property
        //    else if (properties[player.Position].Owner != player && properties[player.Position].Owner != null && player.Money > properties[player.Position].Rent)
        //    {
        //        properties[player.Position].Owner.Money = +properties[player.Position].Rent;
        //        player.Money -= properties[player.Position].Rent;
        //        System.Windows.MessageBox.Show(player.Name + " paid " + properties[player.Position].Rent.ToString() + " to " + properties[player.Position].Owner.Name);
        //    }
        //    else if (properties[player.Position].Owner != null && properties[player.Position].Owner != player && player.Money < properties[player.Position].Rent)
        //    {
        //        System.Windows.MessageBox.Show(player.Name + "Lost !", "Game Over", MessageBoxButton.OK);
        //        players.Remove(player);
        //    }
        //    if (players.Count == 1)
        //    {
        //        System.Windows.MessageBox.Show("Winner is " + player.Name, "Winner", MessageBoxButton.OK);
        //        this.Close();
        //    }

        //    UpdateMoneyTextBox(player);
        //    SwitchToNextPlayer();
        //}

        private async void MovePlayer(Player player, int steps)
        {
            await MovePlayerAndAnimate(player, steps);
            await HandleCellAction(player);
            UpdateMoneyTextBox(player);
            //SendMoveInfo(currentPlayerIndex, steps);
            SwitchToNextPlayer();
        }

        private async Task MovePlayerAndAnimate(Player player, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                player.Position = (player.Position + 1) % 40;
                if (IsPassedGo())
                {
                    player.Money += 1000;
                    SendPassedGoInfo(currentPlayerIndex);
                }
                await AnimatePlayerMove(player);
                await Task.Delay(200);
            }
        }

        private async Task HandleCellAction(Player player)
        {
            if (properties.ContainsKey(player.Position) && properties[player.Position].Owner == null)
            {
                await HandleUnownedProperty(player);
            }
            else if (properties[player.Position].Owner != null && properties[player.Position].Owner != player)
            {
                await HandleRentPayment(player);
            }
            else if (properties[player.Position].Owner == player && player.IsPlayerInJail)
            {
                HandleJailSessions(player);
            }
            UpdateMoneyTextBox(player);
            //SwitchToNextPlayer();
        }

        private async Task HandleUnownedProperty(Player player)
        {
            if (IsPlayerAtGoToJail(player))
            {
                ToJail(player);
                SendJailInfo(currentPlayerIndex);
            }
            else
            {
                await HandleSpecialProperty(player);
            }
        }

        private async Task HandleSpecialProperty(Player player)
        {
            if (CantBuyProperty())
            {
                if (IsQazyna())
                {
                    Random rnd = new Random((int)(Thread.CurrentThread.ManagedThreadId * DateTime.Now.Ticks));
                    int earn = Math.Abs(rnd.Next()) % 500 + 1;
                    player.Money += earn;
                    System.Windows.MessageBox.Show($"You earn {earn}");
                    SendQazynaInfo(currentPlayerIndex,earn);
                }
                else if (IsMumkinshilik())
                {
                    System.Windows.MessageBox.Show("Mumkinshilik");
                }
                else if (IsSalyq())
                {
                    player.Money -= 300;
                    System.Windows.MessageBox.Show("You paid 300 to bank", "Salyq", MessageBoxButton.OK);
                    SendSalyqInfo(currentPlayerIndex);
                }
            }
            else
            {
                await AttemptPropertyPurchase(player);
            }
        }


        private async Task AttemptPropertyPurchase(Player player)
        {
            var result = System.Windows.MessageBox.Show("Purchase?", "Buy", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                BuyProperty(player);
                SendPropertyPurchaseInfo(player.Position);
                //player.proprties.Add(properties[player.Position]);
            }
            else
            {
                StartAuction(player);
            }
        }

        private void BuyProperty(Player player)
        {
            player.Money -= properties[player.Position].Price;
            properties[player.Position].Rectangle.Fill = player.playerColor;
            properties[player.Position].Owner = player;

        }


        private async Task HandleRentPayment(Player player)
        {
            if (player.Money >= properties[player.Position].Rent)
            {
                //properties[player.Position].Owner.Money += properties[player.Position].Rent;
                //player.Money -= properties[player.Position].Rent;
                PayRent(player);
                SendPayRentInfo(currentPlayerIndex);
                System.Windows.MessageBox.Show($"{player.Name} paid {properties[player.Position].Rent} to {properties[player.Position].Owner.Name}");
                UpdateMoneyTextBox(properties[player.Position].Owner);
            }
            else
            {
                System.Windows.MessageBox.Show($"{player.Name} Lost!", "Game Over", MessageBoxButton.OK);
                players.Remove(player);

                if (players.Count == 1)
                {
                    System.Windows.MessageBox.Show($"Winner is {players[0].Name}", "Winner", MessageBoxButton.OK);
                    this.Close();
                }
            }
        }

        private void PayRent(Player player)
        {
            properties[player.Position].Owner.Money += properties[player.Position].Rent;
            player.Money -= properties[player.Position].Rent;
        }


        private void HandleJailSessions(Player player)
        {
            player.SessionsInJail++;
            if (player.SessionsInJail == 2)
            {
                player.IsPlayerInJail = false;
                player.SessionsInJail = 0;
            }
        }

        public bool IsPassedGo()
        {
            return players[currentPlayerIndex].Position == 0;
        }

        public void UpdateMoneyTextBox(Player player)
        {
            foreach (TextBlock block in textBlocks)
            {
                if (block.Name == player.Name)
                {
                    block.Text = player.Money.ToString();
                }
            }
        }

        public bool IsPlayerAtGoToJail(Player player)
        {
            player.IsPlayerInJail = true;
            return player.Position == 30;
        }

        public async void ToJail(Player player)
        {
            for (int i = 0; i < 20; i++)
            {
                player.Position = (player.Position - 1) % 40;
                await AnimatePlayerMove(player);
                await Task.Delay(200);
            }
        }

        private async Task AnimatePlayerMove(Player player)
        {
            double startX = Canvas.GetLeft(player.ellipse);
            double startY = Canvas.GetTop(player.ellipse);

            double endX = cellCenters[player.Position].X - (player.ellipse.Width / 2);
            double endY = cellCenters[player.Position].Y - (player.ellipse.Height / 2);

            double stepSize = 5;
            double steps = Math.Max(Math.Abs(endX - startX), Math.Abs(endY - startY)) / stepSize;

            double stepX = (endX - startX) / steps;
            double stepY = (endY - startY) / steps;

            for (int i = 0; i < steps; i++)
            {
                Canvas.SetLeft(player.ellipse, Canvas.GetLeft(player.ellipse) + stepX);
                Canvas.SetTop(player.ellipse, Canvas.GetTop(player.ellipse) + stepY);
                await Task.Delay(10);
            }
        }
      
        public bool IsSalyq()
        {
            return players[currentPlayerIndex].Position == 38 || players[currentPlayerIndex].Position == 4;
        }

        public bool IsMumkinshilik()
        {
            return players[currentPlayerIndex].Position == 7 || players[currentPlayerIndex].Position == 22 || players[currentPlayerIndex].Position == 37 || players[currentPlayerIndex].Position == 36;

        }
        public bool IsQazyna()
        {
            return players[currentPlayerIndex].Position == 2 || players[currentPlayerIndex].Position == 17 || players[currentPlayerIndex].Position == 33;
        }

        public bool CantBuyProperty()
        {
            return (players[currentPlayerIndex].Position == 38 || players[currentPlayerIndex].Position == 4 || players[currentPlayerIndex].Position == 36 || players[currentPlayerIndex].Position == 7 || players[currentPlayerIndex].Position == 22 || players[currentPlayerIndex].Position == 37 || players[currentPlayerIndex].Position == 2 || players[currentPlayerIndex].Position == 17 || players[currentPlayerIndex].Position == 34 || players[currentPlayerIndex].Position == 0 || players[currentPlayerIndex].Position == 10 || players[currentPlayerIndex].Position == 20 || players[currentPlayerIndex].Position == 30);
        }
        // Auction
        private void StartAuction(Player refusingPlayer)
        {

            int auctionedPropertyPosition = players[currentPlayerIndex].Position;
            List<Player> activePlayers = players.Where(p => p != refusingPlayer).ToList();
            Auction auction = new Auction(this, activePlayers);
            auction.ShowDialog();

            if (IsAuctionOver)
            {
                var winningPlayer = AuctionWinner;
                properties[auctionedPropertyPosition].Owner = winningPlayer;
                winningPlayer.Money -= AuctionOffer;
                properties[auctionedPropertyPosition].Rectangle.Fill = winningPlayer.playerColor;
            }
        }

        // Button MOVE
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random((int)(Thread.CurrentThread.ManagedThreadId * DateTime.Now.Ticks));
            int steps = Math.Abs(rnd.Next()) % 12 + 1;
            btnMove.IsEnabled = false;

            if (IsPlayerAtGoToJail(players[currentPlayerIndex]))
            {
                ToJail(players[currentPlayerIndex]);
            }
            else
            {
                System.Windows.MessageBox.Show(players[currentPlayerIndex].Name + " " + steps.ToString(), "Moves", MessageBoxButton.OK);
                MovePlayer(players[currentPlayerIndex], steps);

                // Отправляем данные о движении другому игроку
                if (currentPlayerIndex == 0)
                {
                    SendMoveToClient(steps);// Сервер отправляет клиенту
                }
                else
                {
                    SendMoveToServer(steps);// Клиент отправляет серверу
                }
            }
        } 



        public TcpClient socPlayer = null;

        //Enter As Server
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FormServer server = new FormServer(this);

            if (server.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //socPlayer = server.socClient;
                Task.Run(() => HandleClientCommunication(socPlayer));
            }
        }


        //Enter As Client
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Form1 client = new Form1(this);
            if (client.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //socPlayer = client.socPlayer;
                Task.Run(() => HandleServerCommunication(socPlayer));
            }
        }

        //Send Move
        private void SendMoveToClient(int steps)
        {
            if (socPlayer != null && socPlayer.Connected)
            {
                string message = $"MOVE;{steps}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        //Send Move
        private void SendMoveToServer(int steps)
        {
            if (socPlayer != null && socPlayer.Connected)
            {
                string message = $"MOVE;{steps}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        //Send Salyq
        private void SendSalyqInfo(int currentIndex)
        {
            if (socPlayer != null && socPlayer.Connected)
            {
                string message = $"SALYQ;{currentIndex}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        //Send Qazyna
        private void SendQazynaInfo(int currentIndex, int earn)
        {
            if(socPlayer != null && socPlayer.Connected)
            {
                string message = $"QAZYNA;{currentIndex};{earn}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        //Send Purchase
        private void SendPropertyPurchaseInfo(int propertyIndex)
        {
            if (socPlayer != null && socPlayer.Connected)
            {
                string message = $"PURCHASE;{currentPlayerIndex};{propertyIndex}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        // Send Pay Rent
        private void SendPayRentInfo(int currentPlayer)
        {
            if (socPlayer != null && socPlayer.Connected)
            {
                string message = $"RENT;{currentPlayer}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        // Send Passed Go
        private void SendPassedGoInfo(int currentPlayer)
        {
            if (socPlayer != null && socPlayer.Connected)
            {
                string message = $"GO;{currentPlayer}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        private void SendJailInfo(int currentPlayer)
        {
            if (socPlayer != null && socPlayer.Connected)
            {
                string message = $"JAIL;{currentPlayer}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                socPlayer.GetStream().Write(data, 0, data.Length);
            }
        }

        private async Task HandleClientCommunication(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] parts = message.Split(';');
                switch (parts[0])
                {
                    case "MOVE":
                        int steps = int.Parse(parts[1]);
                        _ = Dispatcher.Invoke(async () =>
                        {
                            btnMove.IsEnabled = true;
                            await MovePlayerAndAnimate(players[1], steps);
                            SwitchToNextPlayer();
                        }
                        ); // Двигаем игрока на клиенте
                        break;

                    case "PURCHASE":
                        Dispatcher.Invoke(() => { BuyProperty(players[int.Parse(parts[1])]); UpdateMoneyTextBox(players[int.Parse(parts[1])]); });
                        break;
                    case "QAZYNA":
                        int earn = int.Parse(parts[2]);
                        players[int.Parse(parts[1])].Money += earn;
                        Dispatcher.Invoke(() => UpdateMoneyTextBox(players[int.Parse(parts[1])]));
                        break;
                    case "SALYQ":
                        players[int.Parse(parts[1])].Money -= 300;
                        Dispatcher.Invoke(() => UpdateMoneyTextBox(players[int.Parse(parts[1])]));
                        break;
                    case "RENT":
                        PayRent(players[int.Parse(parts[1])]);
                        Dispatcher.Invoke(() => {
                            UpdateMoneyTextBox(players[int.Parse(parts[1])]);
                            UpdateMoneyTextBox(properties[players[int.Parse(parts[1])].Position].Owner);
                        });
                        break;
                    case "JAIL":
                        Dispatcher.Invoke(() => ToJail(players[int.Parse(parts[1])]));
                        break;
                    case "GO":
                        players[int.Parse(parts[1])].Money += 1000;
                        Dispatcher.Invoke(() => UpdateMoneyTextBox(players[int.Parse(parts[1])]));
                        break;

                }
            }
        }
        private async Task HandleServerCommunication(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] parts = message.Split(';');

                switch (parts[0])
                {
                    case "MOVE":
                        int steps = int.Parse(parts[1]);
                        _ = Dispatcher.Invoke(async () =>
                        {
                            btnMove.IsEnabled = true;
                            await MovePlayerAndAnimate(players[0], steps);
                            SwitchToNextPlayer();
                        }
                        ); // Двигаем игрока на клиенте
                        break;
                    case "PURCHASE":
                        Dispatcher.Invoke(() => { BuyProperty(players[int.Parse(parts[1])]); UpdateMoneyTextBox(players[int.Parse(parts[1])]);});
                        break;
                    case "QAZYNA":
                        int earn = int.Parse(parts[2]);
                        players[int.Parse(parts[1])].Money += earn;
                        Dispatcher.Invoke(() =>  UpdateMoneyTextBox(players[int.Parse(parts[1])]));
                        break;
                    case "SALYQ":
                        players[int.Parse(parts[1])].Money -= 300;
                        Dispatcher.Invoke(() => UpdateMoneyTextBox(players[int.Parse(parts[1])]));
                        break;
                    case "RENT":
                        PayRent(players[int.Parse(parts[1])]);
                        Dispatcher.Invoke(() => {
                            UpdateMoneyTextBox(players[int.Parse(parts[1])]);
                            UpdateMoneyTextBox(properties[players[int.Parse(parts[1])].Position].Owner);
                        });
                        break;
                    case "JAIL":
                        Dispatcher.Invoke(() => ToJail(players[int.Parse(parts[1])]));
                        break;
                    case "GO":
                        players[int.Parse(parts[1])].Money += 1000;
                        Dispatcher.Invoke(() => UpdateMoneyTextBox(players[int.Parse(parts[1])]));
                        break;
                }
            }
        }



    }


}   

public class Player
{
    public Ellipse ellipse;
    public Brush playerColor;
    public string Name;
    public int Position;
    public int Money;
    public bool IsPlayerInJail;
    public int SessionsInJail;

    public Player()
    {
        this.Name = "Red";
        this.playerColor = Brushes.Red;
        this.ellipse = new Ellipse
        {
            Width = 15,
            Height = 15,
            Fill = this.playerColor,
            Stroke = Brushes.Black
        };
        Position = 0;
        Money = 1500;
        IsPlayerInJail = false;
        SessionsInJail = 0;
    }

    public Player(string Name, Brush playerColor)
    {
        this.Name = Name;
        this.playerColor = playerColor;
        this.ellipse = new Ellipse
        {
            Width = 15,
            Height = 15,
            Fill = playerColor,
            Stroke = Brushes.Black
        };
        Position = 0;
        Money = 1500;
        IsPlayerInJail = false;
        SessionsInJail = 0;
    }

}

public class Property
{
    public Rectangle Rectangle;
    public int Price;
    public int Rent;
    public Player Owner;

    public Property(Rectangle r, int price, int rent)
    {
        this.Rectangle = r;
        this.Price = price;
        this.Rent = rent;
        Owner = null;
    }
}

