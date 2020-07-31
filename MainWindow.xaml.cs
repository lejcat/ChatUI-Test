using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalMobile_ChatUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		//채팅로그 클래스 초기화
		public Dictionary<string, List<ChatLog>> ChatRooms = new Dictionary<string, List<ChatLog>>();
		public string CurrentRoom;
		public string Username = "User000";	//FOR TEST 사용자 계정명
		//메인 프로그램 실행
        public MainWindow()
        {
            InitializeComponent();
            //비 클라이언트 영역 바인딩 등에 사용되는 옵션
            DataContext = this;

			//TEST 데이터 생성
			TEST_CHAT_CREATER();
			//채팅방 리스트 출력
			Load_Room_List();
        }

		//FOR TEST
		//채팅로그 2차배열 리스트 작성
		private void TEST_CHAT_CREATER()
		{
			//5개 채팅방
			for (int i = 0; i < 5; i++)
			{
				string RoomID = "ROOMID" + i.ToString("000");
				//i번방 채팅로그 리스트 생성
				ChatRooms.Add(RoomID, new List<ChatLog>());
				ChatRooms[RoomID].Add(new ChatLog()
				{ 
					RoomID = RoomID,
					RoomName = "Room" + i.ToString("000"),
					LogDate = DateTime.Now.AddMonths(-30),
					SendUser = "User000",
					ChatText = "Room" + i.ToString("000") + " Long Message Display Test1 " +
					"========================================================================================" +
					"========================================================================================" +
					"========================================================================================"
				});
				ChatRooms[RoomID].Add(new ChatLog()
				{
					RoomID = RoomID,
					RoomName = "Room" + i.ToString("000") + "Name",
					LogDate = DateTime.Now.AddMonths(-29),
					SendUser = "User001",
					ChatText = "Room" + i.ToString("000") + " Long Message Display Test2 " +
					"========================================================================================" +
					"========================================================================================" +
					"========================================================================================"
				});
				for (int j = 0; j < 20; j++)
				{
					ChatRooms[RoomID].Add(new ChatLog()
					{
						RoomID = RoomID,
						RoomName = "Room" + i.ToString("000") + "Name",
						LogDate = DateTime.Now.AddMonths(-5).AddMinutes(-20+j),
						SendUser = "User" + (j%4).ToString("000"),
						ChatText = "RichTextBox Adding 1 - " + j.ToString("000")
					});
				}
				ChatRooms[RoomID].Add(new ChatLog()
				{
					RoomID = RoomID,
					RoomName = "Room" + i.ToString("000"),
					LogDate = DateTime.Now.AddMonths(-4),
					SendUser = "User000",
					ChatText = "Room" + i.ToString("000") + " Long Message Display Test3 " +
					"========================================================================================" +
					"========================================================================================" +
					"========================================================================================"
				});
				ChatRooms[RoomID].Add(new ChatLog()
				{
					RoomID = RoomID,
					RoomName = "Room" + i.ToString("000") + "Name",
					LogDate = DateTime.Now.AddMonths(-3),
					SendUser = "User001",
					ChatText = "Room" + i.ToString("000") + " Long Message Display Test4 " +
					"========================================================================================" +
					"========================================================================================" +
					"========================================================================================"
				});
				for (int j = 0; j < 10; j++)
				{
					ChatRooms[RoomID].Add(new ChatLog()
					{
						RoomID = RoomID,
						RoomName = "Room" + i.ToString("000") + "Name",
						LogDate = DateTime.Now.AddMinutes(-10+j),
						SendUser = "User" + (j % 4).ToString("000"),
						ChatText = "Room" + i.ToString("000") + "RichTextBox Adding 2 - " + j.ToString("000")
					});
				}
			}
		}

		//UI 크기 제한 초기화
		public double sidemax;
		public double SideMax { get { return sidemax; } set { sidemax = value; } }
		public double logmax;
		public double LogMax { get { return logmax; } set { logmax = value; } }
		private void Set_UI_Size()
		{
			//창 크기에 맞춰 UI프레임 크기 제한
			sidemax = ActualWidth / 2;
			logmax = (ActualWidth - SideTab.Width) / 10 * 8;
			if (logmax > 600)
			{
				logmax = 600;
			}
		}
		//채팅방 리스트 출력
		private void Load_Room_List()
		{
			if (ChatRooms.Count > 0)
			{
				foreach (string RoomID in ChatRooms.Keys)
				{
					Grid ChatRoom = new Grid
					{ 
						Name = RoomID,
						Style = (Style)Resources["ChatRoomStyle"]
					};
					ChatRoom.PreviewMouseLeftButtonUp += ChatRoom_PreviewMouseLeftButtonUp;
					//채팅방 타이틀 레이블
					Label label = new Label
					{
						Content = ChatRooms[RoomID][0].RoomName + " (" + RoomID + ")",
						Foreground = new SolidColorBrush(Colors.White),
						FontWeight = FontWeights.Bold,
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Margin = new Thickness(10)
					};
					ChatRoom.Children.Add(label);
					RoomStack.Children.Add(ChatRoom);

					//오름차순 정렬
					ChatRooms[RoomID].Sort(delegate (ChatLog A, ChatLog B)
					{
						if (A.LogDate > B.LogDate)
						{
							return 1;
						}
						else if (A.LogDate < B.LogDate)
						{
							return -1;
						}
						return 0;
					});
				}
			}
		}

		//채팅방 리스트 클릭 이벤트
		private void ChatRoom_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource is Grid grd)
			{
				OpenChat(grd.Name);
			}
		}

		//채팅로그 식별 및 출력
		private void OpenChat(string RoomID)
		{
			try
			{
				//로그 스택 초기화
				ViewStack.Children.Clear();
				//UI 크기 초기화
				Set_UI_Size();
				//ROOMID 값이 넘어오지 않은 경우 중지
				if (RoomID == null)
				{
					return;
				}
				//채팅방 정보 식별 및 로그 정렬
				CurrentRoom = RoomID;
				for (int i = 0; i < ChatRooms[RoomID].Count; i++)
				{
					//일자가 바뀐 경우 표시
					if ((i == 0) || (ChatRooms[RoomID][i].LogDate.ToString("yyyy-MM-dd") != ChatRooms[RoomID][i - 1].LogDate.ToString("yyyy-MM-dd")))
					{
						PrintDate(ChatRooms[RoomID][i].LogDate.ToString("yyyy-MM-dd"));
					}
					//로그 출력
					AddLog(ChatRooms[RoomID][i].ChatText, ChatRooms[RoomID][i].LogDate.ToString("HH:mm"), ChatRooms[RoomID][i].SendUser);
				}
			}
			catch
			{
				OpenChat(RoomID);
			}
			//메시지 출력
		}
		//메시지 출력
		public void AddLog(string chatText, string logTime, string senduser)
		{
			//로그 시간정보 식별
			Label senderInfo = new Label
			{
				Foreground = new SolidColorBrush(Colors.White),
				FontWeight = FontWeights.Bold,
				Content = senduser,
			};
			//로그 출력 개체 초기화
			StackPanel sendLog = new StackPanel
			{
				Orientation = Orientation.Horizontal
			};
			//로그 시간정보 식별
			Label timeInfo = new Label
			{
				Style = Resources["ChatLabel"] as Style,
				VerticalContentAlignment = VerticalAlignment.Bottom,
				Content = logTime
			};
			//로그 객체 생성 및 입력
			TextBox chatLog = new TextBox
			{
				Style = Resources["ChatText"] as Style,
				Text = chatText
			};
			//내 메시지 여부 식별 및 출력위치 정렬
			if (senduser == Username)
			{
				senderInfo.HorizontalAlignment = HorizontalAlignment.Right;
				sendLog.Children.Add(timeInfo);
				sendLog.Children.Add(chatLog);
				sendLog.HorizontalAlignment = HorizontalAlignment.Right;
			}
			else
			{
				senderInfo.HorizontalAlignment = HorizontalAlignment.Left;
				sendLog.Children.Add(chatLog);
				sendLog.Children.Add(timeInfo);
				sendLog.HorizontalAlignment = HorizontalAlignment.Left;
			}
			//스택에 반영
			ViewStack.Children.Add(senderInfo);
			ViewStack.Children.Add(sendLog);
		}
		//날짜정보 출력
		public void PrintDate(string logDate)
		{
			//로그 객체 생성 및 입력
			Paragraph log = new Paragraph(new Run(logDate));
			RichTextBox rtb = new RichTextBox(new FlowDocument(log));
			//출력위치 및 서식 적용
			log.TextAlignment = TextAlignment.Center;
			rtb.HorizontalAlignment = HorizontalAlignment.Center;
			rtb.Width = 100;
			rtb.Padding = new Thickness(0);
			rtb.Margin = new Thickness(10);
			//스택에 반영
			ViewStack.Children.Add(rtb);
		}

		//사이드탭 크기조절
		private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			//Thumb 태그 식별
			var str = (string)((Thumb)sender).Tag;
			//사이드탭 크기 조절
			if (str.Contains("SideTabGrip"))
			{
				//MainWindow의 절반 ~ 사이드탭의 최소 크기 사이에서 그립 범위에 따라 조정
				SideTab.Width = Math.Min(Math.Max(SideTab.MinWidth, SideTab.Width + e.HorizontalChange), sidemax);
				OpenChat(CurrentRoom);
			}
			e.Handled = true;
		}
		//창 크기 변경 함수
		private void Change_Size()
		{
			if (WindowState == WindowState.Normal)
			{
				//최대화
				WindowState = WindowState.Maximized;
				//최대화버튼 변경 > 이전크기
				RectMax.Visibility = Visibility.Hidden;
				RectNor.Visibility = Visibility.Visible;
			}
			else
			{
				//창 크기 복구
				WindowState = WindowState.Normal;
				//이전크기버튼 변경 > 최대화
				RectMax.Visibility = Visibility.Visible;
				RectNor.Visibility = Visibility.Hidden;
			}
		}
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			OpenChat(CurrentRoom);
		}
		//제목표시줄 더블클릭 인식
		private void Dclick_Window(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				Change_Size();
			}
		}
		//제목표시줄 Drag 인식 (제목표시줄에서 마우스가 움직이는 경우)
		private void Drag_Window(object sender, MouseEventArgs e)
		{
			//Drag시 창 이동 (마우스 왼쪽 버튼이 눌려있을 경우)
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				//최대화 상태였을 경우 이전크기 복구
				if (WindowState == WindowState.Maximized)
				{
					//마우스 위치로 윈도우 이동
					var point = PointToScreen(e.MouseDevice.GetPosition(this));
					Left = point.X - (RestoreBounds.Width * 0.5);
					Top = point.Y - 20;
					//창 크기 복구
					WindowState = WindowState.Normal;
					//이전크기버튼 변경 > 최대화
					RectMax.Visibility = Visibility.Visible;
					RectNor.Visibility = Visibility.Hidden;
				}
				DragMove();
				e.Handled = true;
			}
		}
		//최대화or복구 버튼
		private void Button_Maximize(object sender, RoutedEventArgs e)
		{
			Change_Size();
		}
		//최소화 버튼
		private void Button_Minimize(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
		//창닫기 버튼
		private void Button_Close(object sender, RoutedEventArgs e)
		{
			Close();
		}

		//사이드탭 버튼제어
		private void SideTabFold(object sender, MouseButtonEventArgs e)
		{
			if (ShowTabBtn.Visibility == Visibility.Visible)
			{
				ShowTabBtn.Visibility = Visibility.Hidden;
				HideTabBtn.Visibility = Visibility.Visible;
				SideTab.Width = 300;
				SideTab.MinWidth = 100;
			}
			else
			{
				ShowTabBtn.Visibility = Visibility.Visible;
				HideTabBtn.Visibility = Visibility.Hidden;
				SideTab.Width = 0;
				SideTab.MinWidth = 0;
			}
			OpenChat(CurrentRoom);
		}
	}

	//채팅방 클래스
	public class ChatLog
	{
		public string RoomID { get; set; }
		public string RoomName { get; set; }
		public DateTime LogDate { get; set; }
		public string SendUser { get; set; }
		public string ChatText { get; set; }
	}

	//커스텀 버튼 처리
	public class SimpleButton : Button
    {
        public static readonly DependencyProperty MouseOverColorProperty =
            DependencyProperty.Register
            (
                "MouseOverColor", typeof(Brush), typeof(SimpleButton), new PropertyMetadata(new SolidColorBrush(Colors.Red))
            );
        public Brush MouseOverColor
        {
            get { return (Brush)GetValue(MouseOverColorProperty); }
            set { SetValue(MouseOverColorProperty, value); }
        }

    }
}
