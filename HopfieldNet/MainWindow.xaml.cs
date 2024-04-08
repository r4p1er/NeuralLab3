using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HopfieldNet;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int[,] InputImage { get; set; } = new int[10, 10];
    private int[,] OutputImage { get; set; } = new int[10, 10];
    private int[,] Weight { get; set; } = new int[100, 100];
    
    public MainWindow()
    {
        InitializeComponent();

        for (int i = 0; i < 100; ++i)
        {
            for (int j = 0; j < 100; ++j)
            {
                Weight[i, j] = 0;
            }
        }
        
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                InputImage[i, j] = -1;
                OutputImage[i, j] = -1;

                var btnIn = new Button()
                {
                    Name = $"Btn_in_{i}_{j}",
                    Width = 20,
                    Height = 20,
                    Background = Brushes.White,
                    BorderBrush = Brushes.Blue,
                };
                btnIn.Click += Button_Click;
                InputGrid.Children.Add(btnIn);

                var btnOut = new Button()
                {
                    Name = $"Btn_out_{i}_{j}",
                    Width = 20,
                    Height = 20,
                    Background = Brushes.White,
                    BorderBrush = Brushes.Blue
                };
                OutputGrid.Children.Add(btnOut);
            }
        }
    }

    private void Button_Click(object sender, RoutedEventArgs args)
    {
        Button? btn = sender as Button;
        int i = Convert.ToInt32(btn!.Name.Split('_')[2]);
        int j = Convert.ToInt32(btn!.Name.Split('_')[3]);
        InputImage[i, j] *= -1;
        btn.Background = InputImage[i, j] == 1 ? Brushes.Black : Brushes.White;
    }

    private void Button_Clear(object sender, RoutedEventArgs args)
    {
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                InputImage[i, j] = -1;
                OutputImage[i, j] = -1;
            }
        }

        foreach (var item in InputGrid.Children)
        {
            Button? btn = item as Button;
            btn!.Background = Brushes.White;
        }
        
        foreach (var item in OutputGrid.Children)
        {
            Button? btn = item as Button;
            btn!.Background = Brushes.White;
        }
    }
    
    private void Button_Memorize(object sender, RoutedEventArgs args)
    {
        var input = TwoDimArrToOneDimArr(InputImage);
        
        for (int i = 0; i < 100; ++i)
        {
            for (int j = 0; j < 100; ++j)
            {
                if (i == j)
                {
                    Weight[i, j] = 0;
                }
                else
                {
                    Weight[i, j] += input[i] * input[j];
                }
            }
        }
    }

    private int[] TwoDimArrToOneDimArr(int[,] arr)
    {
        var result = new int[arr.Length];

        for (int i = 0, g = 0; i < arr.GetLength(0); ++i)
        {
            for (int j = 0; j < arr.GetLength(1); ++j)
            {
                result[g] = arr[i, j];
                ++g;
            }
        }

        return result;
    }

    private void Button_Recognize(object sender, RoutedEventArgs args)
    {
        int[] prevState = new int[100];
        int[] currentState = TwoDimArrToOneDimArr(InputImage);

        do
        {
            Array.Copy(currentState, prevState, currentState.Length);

            for (int i = 0; i < 100; ++i)
            {
                int sum = 0;

                for (int j = 0; j < 100; ++j)
                {
                    sum += Weight[i, j] * prevState[j];
                }

                currentState[i] = sum >= 0 ? 1 : -1;
            }
        } while (!IsArrsEqual(currentState, prevState));

        for (int i = 0, g = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                OutputImage[i, j] = currentState[g];
                ++g;
            }
        }
        
        foreach (var item in OutputGrid.Children)
        {
            Button? btn = item as Button;
            int i = Convert.ToInt32(btn!.Name.Split('_')[2]);
            int j = Convert.ToInt32(btn!.Name.Split('_')[3]);
            btn.Background = OutputImage[i, j] == 1 ? Brushes.Black : Brushes.White;
        }
    }

    private bool IsArrsEqual(int[] arr1, int[] arr2)
    {
        if (arr1.Length != arr2.Length) return false;

        for (int i = 0; i < arr1.Length; ++i)
        {
            if (arr1[i] != arr2[i]) return false;
        }

        return true;
    }
}