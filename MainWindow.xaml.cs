using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Globalization;
using TangibleAnchoring.Submissions;
using TangibleAnchoring.Config;
using System.Collections.Generic;

namespace TangibleAnchoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        private const string configFile = "Config.xml";
        private readonly Config.Config configData;
        private readonly Submissions.SubmissionData submissionData;
        private Ellipse[] dataPointEllipses;
        private double[] dataPointLeftPosNoZoom, dataPointTopPosNoZoom;
        private Criteria filterCriteria = new Criteria("49", "All Answers");
        Ellipse haloEllipse = new Ellipse();
        bool allowTagging = false;
        bool redrawPointsOnAxisChange = false;
        bool redrawPointsOnXAxisZoom = false;
        bool redrawPointsOnYAxisZoom = false;

        //Following variables are declared to avoid continuous calls to setAxis and DrawPoints
        int prevXMinFacetIndex = 0,
            prevYMinFacetIndex = 0,
            prevAnsChangerFacetIndex = 0,
            prevQuesChangerFacetIndex = 0,
            prevRepViewPtFacetIndex = 0,
            prevDemViewPtFacetIndex = 0,
            prevIndViewPtFacetIndex = 0;

        double prevXDomainStart, prevXDomainEnd, prevYDomainStart, prevYDomainEnd;
        double xDomainStart, xDomainEnd, yDomainStart, yDomainEnd;
        double xRangeStart, xRangeEnd, yRangeStart, yRangeEnd;
        double proposedXAxisLength, proposedYAxisLength;
        double axisChangeTolerance = 1.0;

        Dictionary<string, Ellipse> taggedEllipses = new Dictionary<string, Ellipse>();
        Dictionary<string, Line> xTickLines = new Dictionary<string, Line>();
        Dictionary<string, TextBlock> xTickLabels = new Dictionary<string, TextBlock>();
        Dictionary<string, Line> yTickLines = new Dictionary<string, Line>();
        Dictionary<string, TextBlock> yTickLabels = new Dictionary<string, TextBlock>();

        SolidColorBrush[] viewpointColors = {
                                                (SolidColorBrush)(new BrushConverter().ConvertFrom("#0171d0")),
                                                (SolidColorBrush)(new BrushConverter().ConvertFrom("#63bdf2")),
                                                (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE035")),
                                                (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDC00")),
                                                (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5000")),
                                                (SolidColorBrush)(new BrushConverter().ConvertFrom("#f12727")),
                                                (SolidColorBrush)(new BrushConverter().ConvertFrom("#b30000"))
                                            };
        //WA = Work Area

        //private double WAWidth = System.Windows.SystemParameters.WorkArea.Width;
        //private double WAHeight = System.Windows.SystemParameters.WorkArea.Height;
        //double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        //double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        //private double leftMargin = 100;
        //private double bottomMargin = 50;
        //private double rightMargin = 50;
        //private double topMargin = 50;
        private double YAxisLength, YDomainLength;
        private double XAxisLength, XDomainLength;


        // The following distances specify how far the description textbox should be
        // from the center of the touch device.
        private const double nonTagDescriptionXDistance = 40.0;
        private const double tagDescriptionXDistance = 190.0;
        private const double descriptionYDistance = 30.0;

        /// <summary>
        /// This is where it all begins. First reads the config file, then the 
        /// submissions file, initializes dataPointEllipses, YAxisLength, XAxisLength, and
        /// lastly initializes the visualization.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            configData = new Config.Config(configFile);
            // Plug our item data into our visualizations
            submissionData = new Submissions.SubmissionData(configData.SubmissionsFileUri);
            dataPointEllipses = new Ellipse[submissionData.Submissions.Length];
            dataPointLeftPosNoZoom = new Double[submissionData.Submissions.Length];
            dataPointTopPosNoZoom = new Double[submissionData.Submissions.Length];

            xDomainStart = XAxis.X1;
            xDomainEnd = XAxis.X2;
            //Since initially domain and range are same
            xRangeStart = XAxis.X1;
            xRangeEnd = XAxis.X2;

            yDomainStart = YAxis.Y2;
            yDomainEnd = YAxis.Y1;
            yRangeStart = YAxis.Y2;
            yRangeEnd = YAxis.Y1;

            //YAxisLength = YAxis.Y2 - YAxis.Y1;
            //XAxisLength = XAxis.X2 - XAxis.X1;

            //Below calculations are based on origin that is top left corner
            YAxisLength = yRangeStart - yRangeEnd;
            YDomainLength = YAxisLength;
            XAxisLength = xRangeEnd - xRangeStart;
            XDomainLength = XAxisLength;
            proposedXAxisLength = XAxisLength;
            proposedYAxisLength = YAxisLength;

            InitScatterplot("88", "All Answers", "46", "4");
        }

        /// <summary>
        /// Initializes the question and answer text, sets the y and x axis, and draws x-ticks, y-ticks, and data points.
        /// </summary>
        /// <param name="quesId">Current question id.</param>
        /// <param name="ansId">Current answer id for the chosen question.</param>
        /// <param name="xAxisQuesId">Initial x axis variable.</param>
        /// <param name="yAxisQuesId">Initial y axis variable.</param>
        private void InitScatterplot(string quesId, string ansId, string xAxisQuesId, string yAxisQuesId)
        {
            SetQuestion(quesId);
            // Initialize the XAxis
            SetXAxis(xAxisQuesId);

            // Initialize the YAxis
            SetYAxis(yAxisQuesId);

            DrawPoints(1, 1);
            DrawXTicks();
            DrawYTicks();
        }

        #region Helper_Functions
        /// <summary>
        /// Shows a log message on the top left.
        /// </summary>
        /// <param name="message">string message</param>
        private void LogMsg(string message)
        {
            LogMessageLabel.Content = message;
        }

        /// <summary>
        /// Set the question text and id for updating the details associated with a data point.
        /// </summary>
        /// <param name="questionId">Id for the current question.</param>
        private void SetQuestion(string questionId)
        {
            //get the question questionText
            string qText = configData.FindQuestionFromId(questionId).QuestionText;
            CurrentQuestion.Content = qText;
            CurrentQuestion.Uid = questionId;

            //Re-initialize the current answer to All Answers
            CurrentAnswer.Content = "All Answers";
            // LogMsg(qText);
        }

        /// <summary>
        /// Set the answer text and id for data points to be drawn correctly.
        /// </summary>
        /// <param name="answerId">Id of the answer for current question.</param>
        /// <param name="questionId">Id of the current question that sets the context.</param>
        private void SetAnswer(string questionId, string answerId)
        {
            //get the question questionText
            string qText = configData.FindQuestionFromId(questionId).QuestionText;

            if (answerId == "")
            {
                CurrentAnswer.Content = "All Answers";
            }
            else
            {
                string aText = configData.FindAnswerFromQuesIdAnsId(questionId, answerId).AnswerText;
                CurrentAnswer.Content = aText;
                CurrentAnswer.Uid = answerId;
            }

            // LogMsg(qText);
        }

        /// <summary>
        /// Set the label and context for ticks and data points to be drawn correctly.
        /// </summary>
        /// <param name="xAxisQuesId">Question id that corresponds to the x axis variable</param>
        private void SetXAxis(string xAxisQuesId)
        {
            XAxisLabel.Content = configData.FindQuestionFromId(xAxisQuesId).QuestionText;
            XAxisLabel.Uid = xAxisQuesId;
            XAxis.Uid = xAxisQuesId;
        }

        /// <summary>
        /// Set the label and context for ticks and data points to be drawn correctly.
        /// </summary>
        /// <param name="yAxisQuesId">Question id that corresponds to the y axis variable</param>
        private void SetYAxis(string yAxisQuesId)
        {
            YAxisLabel.Content = configData.FindQuestionFromId(yAxisQuesId).QuestionText;
            YAxisLabel.Uid = yAxisQuesId;
            YAxis.Uid = yAxisQuesId;
        }

        private void UpdateYDomainLength()
        {
            YDomainLength = yDomainStart - yDomainEnd;
        }

        private void UpdateXDomainLength()
        {
            XDomainLength = xDomainEnd - xDomainStart;
        }

        private void BackupDataPointPositions() 
        {
            for (int i = 0; i < dataPointEllipses.Length; i++)
            { 
                //TODO verify whether the left and top positions are correct
                dataPointLeftPosNoZoom[i] = Canvas.GetLeft(dataPointEllipses[i]);
                dataPointTopPosNoZoom[i] = Canvas.GetTop(dataPointEllipses[i]);
            }
        }

        //private Line getTickFromId(string axis, string answerId)
        //{
        //    Line newTick = new Line();
        //    switch (axis)
        //    {
        //        case "xaxis":
        //            LogMsg("Answerid: "+answerId);
        //            if (MainCanvas.FindName("XTick_2") != null)
        //            {
        //                newTick = (Line) MainCanvas.FindName("XTick_" + answerId);
        //            }
        //            break;
        //        case "yaxis":
        //            //This is needed only if the variable is not a range otherwise a direct calculation of x, y is required
        //            //newTick = (Line)MainCanvas.FindName("YTick_" + answerId);
        //            break;
        //        default:
        //            break;
        //    }


        //    return newTick;
        //}
        #endregion Helper_Functions

        #region Drawing_Functions
        /// <summary>
        /// Draw ticks and corresponding labels for Y axis based on the type of variable selected.
        /// </summary>
        private void DrawYTicks()
        {
            string qId;
            SolidColorBrush tickLabelColorBrush = new SolidColorBrush();
            tickLabelColorBrush.Color = SurfaceColors.ControlBorderColor;

            qId = YAxis.Uid;
            //Clear previous yTicks before starting afresh
            foreach (string yTickKey in yTickLines.Keys)
            {
                string index = yTickKey.Split('_')[1];
                MainCanvas.Children.Remove(yTickLines[yTickKey]);
                MainCanvas.Children.Remove(yTickLabels["YTickLabel_" + index]);
            }
            yTickLabels.Clear();
            yTickLines.Clear();

            Question ques = configData.FindQuestionFromId(qId);
            if (ques.Answers != null)
            {
                int numAnswers = ques.Answers.Length;
                double step = YAxisLength / numAnswers;
                for (int index = 0; index < numAnswers; index++)
                {
                    Line tick = new Line();
                    tick.Name = "YTick_" + index;
                    tick.Stroke = System.Windows.Media.Brushes.DarkSlateGray;
                    tick.X1 = YAxis.X1 - 10.00;
                    tick.Y1 = YAxis.Y1 + (numAnswers - index) * step;
                    tick.X2 = YAxis.X1 + 10.00;
                    tick.Y2 = tick.Y1;
                    tick.StrokeThickness = 1;
                    //MainCanvas.RegisterName(tick.Name, tick);
                    MainCanvas.Children.Add(tick);


                    TextBlock tLabel = new TextBlock();
                    tLabel.Name = "YTickLabel_" + index;
                    tLabel.Foreground = tickLabelColorBrush;
                    tLabel.FontSize = 18;
                    tLabel.Width = 80;
                    tLabel.Text = ques.Answers[index].AnswerText;
                    tLabel.TextWrapping = TextWrapping.WrapWithOverflow;

                    //Next three lines allow rotation of tick labels
                    //RotateTransform myTransform = new RotateTransform();
                    //myTransform.Angle = -90;
                    //tLabel.RenderTransform = myTransform;
                    tLabel.SetValue(Canvas.LeftProperty, tick.X1 - 80);
                    tLabel.SetValue(Canvas.TopProperty, tick.Y1 - step / 2 - tLabel.FontSize);
                    //MainCanvas.RegisterName(tLabel.Name, tLabel);
                    MainCanvas.Children.Add(tLabel);

                    yTickLines.Add(tick.Name, tick);
                    yTickLabels.Add(tLabel.Name, tLabel);
                }
            }
            else if (ques.AnswerRange != null)
            {
                string[] answerRangeArr = ques.AnswerRange.Split(',');
                //ASSUMPTION start and end value are integers only
                int startValue = int.Parse(answerRangeArr[0]);
                int endValue = int.Parse(answerRangeArr[1]);
                int increment = int.Parse(answerRangeArr[2]);
                int range = endValue - startValue;

                if (range <= 10)
                {
                    double step = YAxisLength / range;
                    for (int index = startValue; index <= endValue; index++)
                    {
                        Line tick = new Line();
                        tick.Name = "YTick_" + index;
                        tick.Stroke = System.Windows.Media.Brushes.DarkSlateGray;
                        tick.X1 = YAxis.X1 - 10.00;
                        tick.Y1 = YAxis.Y1 + (endValue - index) * step;
                        tick.X2 = YAxis.X1 + 10.00;
                        tick.Y2 = tick.Y1;
                        tick.StrokeThickness = 1;
                        MainCanvas.Children.Add(tick);
                        //MainCanvas.RegisterName(tick.Name, tick);

                        TextBlock tLabel = new TextBlock();
                        tLabel.Name = "YTickLabel_" + index;
                        tLabel.Foreground = tickLabelColorBrush;
                        tLabel.FontSize = 18;
                        tLabel.Text = index.ToString();
                        tLabel.SetValue(Canvas.LeftProperty, tick.X1 - 20);
                        tLabel.SetValue(Canvas.TopProperty, tick.Y1 - 20);
                        MainCanvas.Children.Add(tLabel);
                        // MainCanvas.RegisterName(tLabel.Name, tLabel);
                        yTickLines.Add(tick.Name, tick);
                        yTickLabels.Add(tLabel.Name, tLabel);
                    }
                }
                else if (range > 10)
                {
                    double step = YAxisLength / 10;
                    //TODO take into account the increment if it is different than 1
                    for (int index = 0; index < 10; index++)
                    {
                        Line tick = new Line();
                        tick.Name = "YTick_" + index;
                        tick.Stroke = System.Windows.Media.Brushes.DarkSlateGray;
                        tick.X1 = YAxis.X1 - 10.00;
                        tick.Y1 = YAxis.Y1 + (10 - index) * step;
                        tick.X2 = YAxis.X1 + 10.00;
                        tick.Y2 = tick.Y1;
                        tick.StrokeThickness = 1;
                        MainCanvas.Children.Add(tick);
                        //MainCanvas.RegisterName(tick.Name, tick);

                        TextBlock tLabel = new TextBlock();
                        tLabel.Name = "YTickLabel_" + index;
                        tLabel.Foreground = tickLabelColorBrush;
                        tLabel.FontSize = 18;
                        tLabel.Text = ((range / 10.00) * index).ToString();
                        tLabel.SetValue(Canvas.LeftProperty, tick.X1 - 20);
                        tLabel.SetValue(Canvas.TopProperty, tick.Y1 - 20);
                        MainCanvas.Children.Add(tLabel);
                        //MainCanvas.RegisterName(tLabel.Name, tLabel);
                        yTickLines.Add(tick.Name, tick);
                        yTickLabels.Add(tLabel.Name, tLabel);
                    }
                }
            }
        }

        /// <summary>
        /// Draw ticks and corresponding labels for X axis based on the type of variable selected.
        /// </summary>
        private void DrawXTicks()
        {
            string qId;
            SolidColorBrush tickLabelColorBrush = new SolidColorBrush();
            tickLabelColorBrush.Color = SurfaceColors.ControlBorderColor;

            //Clear previous xTicks before starting afresh
            foreach (string xTickKey in xTickLines.Keys)
            {
                string index = xTickKey.Split('_')[1];
                MainCanvas.Children.Remove(xTickLines[xTickKey]);
                MainCanvas.Children.Remove(xTickLabels["XTickLabel_" + index]);
            }
            xTickLabels.Clear();
            xTickLines.Clear();

            qId = XAxis.Uid;
            Question ques = configData.FindQuestionFromId(qId);
            int numAnswers = ques.Answers.Length;
            double step = XAxisLength / numAnswers;
            //Since we want to be able to spread the data points that align
            // in a line for a value, the lowest value of XAxis will start from second tick

            for (int index = 0; index < numAnswers; index++)
            {

                Line tick = new Line();
                tick.Name = "XTick_" + index;
                tick.Stroke = System.Windows.Media.Brushes.DarkSlateGray;
                tick.X1 = XAxis.X1 + index * step;
                tick.Y1 = XAxis.Y1 - 10.00;
                tick.X2 = tick.X1;
                tick.Y2 = XAxis.Y1 + 10.00;
                tick.StrokeThickness = 1;
                //MainCanvas.RegisterName(tick.Name, tick);
                MainCanvas.Children.Add(tick);


                TextBlock tLabel = new TextBlock();
                tLabel.Name = "XTickLabel_" + index;
                tLabel.Foreground = tickLabelColorBrush;
                tLabel.FontSize = 18;
                tLabel.Text = ques.Answers[index].AnswerText;
                tLabel.SetValue(Canvas.LeftProperty, tick.X1 + step / 2 - (tLabel.Text.Length * (tLabel.FontSize * 0.75)) / 2);
                tLabel.SetValue(Canvas.TopProperty, tick.Y1 + 20);
                //MainCanvas.RegisterName(tLabel.Name, tLabel);
                MainCanvas.Children.Add(tLabel);

                xTickLines.Add(tick.Name, tick);
                xTickLabels.Add(tLabel.Name, tLabel);
            }
        }

        /// <summary>
        /// Logic for drawing data points. It is dependant on current 
        /// question, current answer and the two axes.
        /// </summary>
        /// <param name="submissionData">collection of points</param>
        private void DrawPoints(int xAxisZoom, int yAxisZoom)
        {
            if (submissionData != null)
            {
                int numPoints = submissionData.Submissions.Length;
                Random r = new Random();
                if (!redrawPointsOnAxisChange && !redrawPointsOnXAxisZoom && !redrawPointsOnYAxisZoom) //first time draw
                {
                    for (int index = 0; index < numPoints; index++)
                    {
                        dataPointEllipses[index] = new Ellipse();
                        Submissions.Submission sData = submissionData.Submissions[index];
                        string answerIdForXAxis = sData.FindResponseFromQuestionId(XAxis.Uid).AnswerId;
                        int rangeXAxis = configData.FindQuestionFromId(XAxis.Uid).Answers.Length;
                        //double leftPosition = getTickFromId("xaxis", answerIdForXAxis).X1 + r.Next(20) ;
                        int xTickInterval = (int)(XAxisLength / rangeXAxis) * xAxisZoom;
                        double leftPosition = YAxis.X1 + xTickInterval * (int.Parse(answerIdForXAxis) - 1) + r.Next(xTickInterval);

                        /******** Calculation for topPosition ********/
                        //If there is a range variable on YAxis then we will have to scale as per range and YAxisLength
                        //ASSUMPTION That the variable is a range, else just follow the example of leftPosition
                        //ASSUMPTION Hard coding the YAxis vairable to be Age

                        string quesIdForYAxis = YAxis.Uid;
                        double topPosition = 0.0, rangeYAxis = 0.0;
                        switch (quesIdForYAxis)
                        {
                            case "4": //Age
                                string[] answerRangeArr = configData.FindQuestionFromId(quesIdForYAxis).AnswerRange.Split(',');
                                rangeYAxis = double.Parse(answerRangeArr[1]) - double.Parse(answerRangeArr[0]);
                                //For general case use the answerId found in next statement to find the x,y position
                                //string answerIdForYAxis = sData.FindResponseFromQuestionId(YAxis.Uid).AnswerId;
                                //For Age, get direct value of age from sData
                                int answerValue = sData.Age;
                                topPosition = YAxis.Y2 - (double)answerValue * (YAxisLength / rangeYAxis) - 10;

                                break;
                            case "5": //Obama or Romney
                                string answerIdForYAxis = sData.FindResponseFromQuestionId(YAxis.Uid).AnswerId;
                                rangeYAxis = configData.FindQuestionFromId(YAxis.Uid).Answers.Length;
                                //double leftPosition = getTickFromId("xaxis", answerIdForXAxis).X1 + r.Next(20) ;
                                int yTickInterval = (int)(YAxisLength / rangeYAxis) * yAxisZoom;
                                topPosition = YAxis.Y2 - yTickInterval * (int.Parse(answerIdForYAxis) - 1) - 10;
                                break;
                        }

                        dataPointEllipses[index].SetValue(Canvas.LeftProperty, leftPosition);
                        dataPointEllipses[index].SetValue(Canvas.TopProperty, topPosition);
                        dataPointEllipses[index].Name = "data_" + index;
                        dataPointEllipses[index].Uid += index;

                        //Dynamic assignment of touch event handler
                        dataPointEllipses[index].TouchEnter += new EventHandler<TouchEventArgs>(DataPointTouchEnter);
                        dataPointEllipses[index].TouchLeave += new EventHandler<TouchEventArgs>(DataPointTouchLeave);
                        dataPointEllipses[index].Height = 20;
                        dataPointEllipses[index].Width = 20;

                        // AnswerId - 1 because viewpoints are numbered from 1..7 
                        dataPointEllipses[index].Fill = viewpointColors[int.Parse(sData.Responses[0].AnswerId) - 1];
                        dataPointEllipses[index].Opacity = 0.6;
                        MainCanvas.Children.Add(dataPointEllipses[index]);
                        
                    }
                    BackupDataPointPositions();
                }
                else //redrawing
                {
                    if (redrawPointsOnAxisChange)
                    {
                        for (int index = 0; index < numPoints; index++)
                        {
                            Submissions.Submission sData = submissionData.Submissions[index];
                            string answerIdForXAxis = sData.FindResponseFromQuestionId(XAxis.Uid).AnswerId;
                            int rangeXAxis = configData.FindQuestionFromId(XAxis.Uid).Answers.Length;
                            //double leftPosition = getTickFromId("xaxis", answerIdForXAxis).X1 + r.Next(20) ;
                            int xTickInterval = (int)(XAxisLength / rangeXAxis) * xAxisZoom;
                            double leftPosition = YAxis.X1 + xTickInterval * (int.Parse(answerIdForXAxis) - 1) + r.Next(xTickInterval);


                            string quesIdForYAxis = YAxis.Uid;
                            double topPosition = 0.0, rangeYAxis = 0.0;
                            switch (quesIdForYAxis)
                            {
                                case "4": //Age
                                    string[] answerRangeArr = configData.FindQuestionFromId(quesIdForYAxis).AnswerRange.Split(',');
                                    rangeYAxis = double.Parse(answerRangeArr[1]) - double.Parse(answerRangeArr[0]);
                                    //For general case use the answerId found in next statement to find the x,y position
                                    //string answerIdForYAxis = sData.FindResponseFromQuestionId(YAxis.Uid).AnswerId;
                                    //For Age, get direct value of age from sData
                                    int answerValue = sData.Age;

                                    topPosition = YAxis.Y2 - (double)answerValue * (YAxisLength / rangeYAxis) - 10;

                                    break;
                                case "5": //Obama or Romney

                                    string answerIdForYAxis = sData.FindResponseFromQuestionId(YAxis.Uid).AnswerId;
                                    rangeYAxis = configData.FindQuestionFromId(YAxis.Uid).Answers.Length;
                                    //double leftPosition = getTickFromId("xaxis", answerIdForXAxis).X1 + r.Next(20) ;
                                    int yTickInterval = (int)(YAxisLength / rangeYAxis) * yAxisZoom;
                                    topPosition = YAxis.Y2 - yTickInterval * (int.Parse(answerIdForYAxis) - 1) - 10 - r.Next(yTickInterval);
                                    break;
                            }
                            dataPointEllipses[index].SetValue(Canvas.LeftProperty, leftPosition);
                            dataPointEllipses[index].SetValue(Canvas.TopProperty, topPosition);

                        }
                        BackupDataPointPositions();
                        redrawPointsOnAxisChange = false;
                    }
                    else if (redrawPointsOnXAxisZoom)
                    {
                        double zoomFactor = XAxisLength / (xDomainEnd - xDomainStart);
                        double xStartShift = xDomainStart - xRangeStart;
                        double xEndShift = xRangeEnd - xDomainEnd;
                        for (int index = 0; index < numPoints; index++)
                        {
                            double currentLeftPosition = dataPointLeftPosNoZoom[index];
                            double newLeftPosition = (currentLeftPosition - 160)* zoomFactor - xStartShift*zoomFactor + 160;
                            //LogMsg("xDSt: " + xDomainStart +" zoom:"+zoomFactor);
                            //LogMsg("XAxisEnd: " + (1900*zoomFactor - xStartShift*zoomFactor) + " XShift: "+xStartShift);

                            Submissions.Submission sData = submissionData.Submissions[index];
                            string answerIdForXAxis = sData.FindResponseFromQuestionId(XAxis.Uid).AnswerId;
                            int rangeXAxis = configData.FindQuestionFromId(XAxis.Uid).Answers.Length;
                            //double leftPosition = getTickFromId("xaxis", answerIdForXAxis).X1 + r.Next(20) ;
                            int xTickInterval = (int)(XAxisLength / rangeXAxis) * xAxisZoom;
                            //double leftPosition = YAxis.X1 + xTickInterval * (int.Parse(answerIdForXAxis) - 1) + r.Next(xTickInterval);


                            string quesIdForYAxis = YAxis.Uid;
                            double topPosition = 0.0, rangeYAxis = 0.0;
                            switch (quesIdForYAxis)
                            {
                                case "4": //Age
                                    string[] answerRangeArr = configData.FindQuestionFromId(quesIdForYAxis).AnswerRange.Split(',');
                                    rangeYAxis = double.Parse(answerRangeArr[1]) - double.Parse(answerRangeArr[0]);
                                    //For general case use the answerId found in next statement to find the x,y position
                                    //string answerIdForYAxis = sData.FindResponseFromQuestionId(YAxis.Uid).AnswerId;
                                    //For Age, get direct value of age from sData
                                    int answerValue = sData.Age;

                                    topPosition = YAxis.Y2 - (double)answerValue * (YAxisLength / rangeYAxis) - 10;

                                    break;
                                case "5": //Obama or Romney

                                    string answerIdForYAxis = sData.FindResponseFromQuestionId(YAxis.Uid).AnswerId;
                                    rangeYAxis = configData.FindQuestionFromId(YAxis.Uid).Answers.Length;
                                    //double leftPosition = getTickFromId("xaxis", answerIdForXAxis).X1 + r.Next(20) ;
                                    int yTickInterval = (int)(YAxisLength / rangeYAxis) * yAxisZoom;
                                    topPosition = YAxis.Y2 - yTickInterval * (int.Parse(answerIdForYAxis) - 1) - 10 - r.Next(yTickInterval);
                                    break;
                            }
                            dataPointEllipses[index].SetValue(Canvas.LeftProperty, newLeftPosition);
                            dataPointEllipses[index].SetValue(Canvas.TopProperty, topPosition);
                            if (newLeftPosition < xRangeStart || newLeftPosition >= xRangeEnd)
                            {
                                dataPointEllipses[index].Visibility = System.Windows.Visibility.Hidden;
                            } 
                        }
                        redrawPointsOnXAxisZoom = false;
                    }
                    else if (redrawPointsOnYAxisZoom)
                    {
                        redrawPointsOnYAxisZoom = false;
                    }

                    
                }
                
            }
        }
        #endregion Drawing_Functions

        #region Touch_Handlers_N_Helpers
        /// <summary>
        /// Touch Event handler for dynamically added data point.
        /// </summary>
        /// <param name="sender">element that invokes this handler (e.g an Ellipse)</param>
        /// <param name="e">touch event information such type of device</param>
        private void DataPointTouchLeave(object sender, TouchEventArgs e)
        {
            Ellipse senderEllipse = sender as Ellipse;
            MainCanvas.Children.Remove(haloEllipse);
            Description.Visibility = Visibility.Hidden;
            ConnectingLine.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Touch Event handler for dynamically added data point.
        /// </summary>
        /// <param name="sender">element that invokes this handler (e.g an Ellipse)</param>
        /// <param name="e">touch event information such type of device</param>
        private void DataPointTouchEnter(object sender, TouchEventArgs e)
        {
            Ellipse senderEllipse = sender as Ellipse;
            int submissionIndex = int.Parse(senderEllipse.Name.Split('_')[1]);
            if (allowTagging)
            {
                if (!taggedEllipses.ContainsKey("tag_" + submissionIndex)) //if the ellipse has not been tagged already
                {
                    Ellipse tagEllipse = new Ellipse();
                    tagEllipse.SetValue(Canvas.LeftProperty, Canvas.GetLeft(senderEllipse) - senderEllipse.Width / 4);
                    tagEllipse.SetValue(Canvas.TopProperty, Canvas.GetTop(senderEllipse) - senderEllipse.Height / 4);
                    tagEllipse.Height = 30;
                    tagEllipse.Width = 30;
                    tagEllipse.Stroke = Brushes.Aqua;
                    tagEllipse.Uid = "tag_" + submissionIndex;
                    tagEllipse.StrokeThickness = 2;
                    MainCanvas.Children.Add(tagEllipse);
                    taggedEllipses.Add(tagEllipse.Uid, tagEllipse);
                    //This will produce details on demand
                    
                }
                Submission submission = submissionData.Submissions[submissionIndex];
                UpdateDescription(RootGrid, e.TouchDevice, submission, true);
            }
            else
            {
                haloEllipse.SetValue(Canvas.LeftProperty, Canvas.GetLeft(senderEllipse) - senderEllipse.Width / 4);
                haloEllipse.SetValue(Canvas.TopProperty, Canvas.GetTop(senderEllipse) - senderEllipse.Height / 4);
                haloEllipse.Height = 30;
                haloEllipse.Width = 30;
                haloEllipse.Stroke = Brushes.AntiqueWhite;
                haloEllipse.StrokeThickness = 2;
                MainCanvas.Children.Remove(haloEllipse); //FIX: Next line throws ArgumentException if another haloEllipse exists while new is added.
                MainCanvas.Children.Add(haloEllipse);


                Submission submission = submissionData.Submissions[submissionIndex];
                if (senderEllipse != null)
                {
                    //LogMsg(submission.UserId);
                    LogMsg("Left:" + dataPointLeftPosNoZoom[submissionIndex] + " Top:" + dataPointTopPosNoZoom[submissionIndex]);

                    //Interactively remove elements that have been touched (could make for a game)
                    //MainCanvas.Children.Remove(senderEllipse);
                }

                //This will produce details on demand
                UpdateDescription(RootGrid, e.TouchDevice, submission, true);
            }


            // Capture to the ellipse.  
            //e.TouchDevice.Capture(senderEllipse);

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values.  
            // Each value has a facetRange of 0-255.
            mySolidColorBrush.Color = SurfaceColors.Accent2Color;

            //Used to change color of touched elements.
            //senderEllipse.Fill = mySolidColorBrush;


            // Mark this event as handled.  
            e.Handled = true;
        }

        /// <summary>
        /// Gets a string that describes the type of touch device on the surface
        /// </summary>
        /// <param name="touchDevice">the touch device to be examined</param>
        /// <returns>a string that describes the type of touch device</returns>
        private static string GetTouchDeviceTypeString(TouchDevice touchDevice)
        {
            if (touchDevice.GetTagData() != TagData.None)
            {
                return "Tag";
            }
            if (touchDevice.GetIsFingerRecognized())
            {
                return "Finger";
            }

            return "Blob";
        }

        /// <summary>
        /// Update the questionText description with the most recent property values. Position
        /// the textbox so that it does not go offscreen (outside parentGrid). Also
        /// position the connecting line between the touch device and the textbox.
        /// </summary>
        /// <param name="parentGrid">the container for this diagram-
        /// description questionText will not go outside of this container's bounds</param>
        /// <param name="touchDevice">the touch device to diagram</param>
        /// <param name="showTouchDeviceInfo">Whether or not the touch device info will be visible</param>
        private void UpdateDescription(Grid parentGrid, TouchDevice touchDevice, Submission submission, bool showTouchDeviceInfo)
        {
            // Show or hide the touchDevice info based on showTouchDeviceInfo
            Description.Visibility = showTouchDeviceInfo ? Visibility.Visible : Visibility.Hidden;
            ConnectingLine.Visibility = showTouchDeviceInfo ? Visibility.Visible : Visibility.Hidden;

            if (!showTouchDeviceInfo)
            {
                // Don't need to do the calculations if info isn't going to be shown
                return;
            }

            Point position = touchDevice.GetPosition(parentGrid);
            Rect bounds = new Rect(0, 0, parentGrid.ActualWidth, parentGrid.ActualHeight);
            // Determine where around the touchDevice the description should be displayed.
            // The default position is above and to the left.
            bool isAbove = true;
            bool isLeft = true;

            // Description questionText for tags is different than non-tags
            double descriptionXDistance;
            bool isTag = touchDevice.GetIsTagRecognized();

            if (isTag)
            {
                descriptionXDistance = tagDescriptionXDistance;
            }
            else
            {
                descriptionXDistance = nonTagDescriptionXDistance;
            }

            // Put description below touchDevice if default location is out of bounds.
            Rect upperLeftBounds = GetDescriptionBounds(position, isAbove, isLeft, descriptionXDistance, descriptionYDistance);
            if (upperLeftBounds.Top < bounds.Top)
            {
                isAbove = false;
            }

            // Put description to the right of the touchDevice if default location is out of bounds.
            if (upperLeftBounds.Left < bounds.Left)
            {
                isLeft = false;
            }

            // Calculate the final bounds that will be used for the textbox position
            // based on the updated isAbove and isLeft values.
            Rect finalBounds = GetDescriptionBounds(position, isAbove, isLeft, descriptionXDistance, descriptionYDistance);
            Canvas.SetLeft(Description, finalBounds.Left);
            Canvas.SetTop(Description, finalBounds.Top);

            // Set the justification of the type in the textbox based
            // on which side of the touchDevice the textbox is on.
            if (isLeft)
            {
                Description.TextAlignment = TextAlignment.Right;
            }
            else
            {
                Description.TextAlignment = TextAlignment.Left;
            }

            // Create the description string.
            StringBuilder descriptionText = new StringBuilder();
            //Show demographic info for submission data
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Respondent questionId: {0}", submission.UserId));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Age: {0}", submission.Age.ToString()));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Location: {0}, {1}", submission.Latitude.ToString(), submission.Longitude.ToString()));
            //TODO Make the text dynamic as per axis or static based on what susan needs

            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, XAxisLabel.Content + ": {0}", configData.FindQuestionFromId(XAxis.Uid).Answers[int.Parse(submission.FindResponseFromQuestionId(XAxis.Uid).AnswerId) - 1].AnswerText));
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, YAxisLabel.Content + ": {0}", (YAxis.Uid == "4") ? submission.Age.ToString() : configData.FindQuestionFromId(YAxis.Uid).Answers[int.Parse(submission.FindResponseFromQuestionId(YAxis.Uid).AnswerId) - 1].AnswerText));
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Affiliation: {0}", configData.FindAnswerFromQuesIdAnsId("49", submission.FindResponseFromQuestionId("49").AnswerId).AnswerText));
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Response: {0}", configData.FindAnswerFromQuesIdAnsId(CurrentQuestion.Uid, submission.FindResponseFromQuestionId(CurrentQuestion.Uid).AnswerId).AnswerText));

            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "RecognizedTypes: {0}", GetTouchDeviceTypeString(touchDevice)));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "QuestionId: {0}", touchDevice.QuestionId));

            //// Use the "f1" format specifier to limit the amount of decimal positions shown.
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "X: {0}", position.X.ToString("f1", CultureInfo.InvariantCulture)));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Y: {0}", position.Y.ToString("f1", CultureInfo.InvariantCulture)));

            // Display "null" for Orientation if the touchDevice does not have an orientation value.
            //string orientationString;
            //double? touchDeviceOrientation = touchDevice.GetOrientation(parentGrid);
            //if (touchDeviceOrientation == null)
            //{
            //    orientationString = "null";
            //}
            //else
            //{
            //    orientationString = ((double)touchDeviceOrientation).ToString("f1", CultureInfo.InvariantCulture);
            //}
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Orientation: {0}", orientationString));

            //if (touchDevice.GetTagData() != TagData.None)
            //{
            //    //descriptionText.AppendLine("Schema: 0x" + touchDevice.GetTagData().Schema.ToString("x8", CultureInfo.InvariantCulture));
            //    //descriptionText.AppendLine("Series:  0x" + touchDevice.GetTagData().Series.ToString("x16", CultureInfo.InvariantCulture));
            //    //descriptionText.AppendLine("ExtendedValue: 0x" + touchDevice.GetTagData().ExtendedValue.ToString("x16", CultureInfo.InvariantCulture));
            //    descriptionText.AppendLine("Value:  0x" + touchDevice.GetTagData().Value.ToString("x4", CultureInfo.InvariantCulture));
            //}

            // Update the description textbox.
            Description.Text = descriptionText.ToString();

            // Update the line that connects the touchDevice to the description textbox.
            double x2;
            if (isLeft)
            {
                x2 = finalBounds.Right;
            }
            else
            {
                x2 = finalBounds.Left;
            }
            // Position (X1,Y1) is the center of the touchDevice.
            // Position (X2,Y2) is the edge of the description questionText box.
            ConnectingLine.X1 = position.X;
            ConnectingLine.Y1 = position.Y;
            ConnectingLine.X2 = x2;
            ConnectingLine.Y2 = finalBounds.Top + finalBounds.Height * 0.5;
        }

        /// <summary>
        /// Calculate the bounds of the textbox for the specified location and quadrant.
        /// </summary>
        /// <param name="position">position of the touch device to diagram</param>
        /// <param name="isAbove">specifies if the requested bounds is above or below the touch device position</param>
        /// <param name="isLeft">specifies if the requested bounds is right or left of the touch device position</param>
        /// <param name="xDistance">the x distance from the touch evice position</param>
        /// <param name="yDistance">the y distance from the touch device position</param>
        /// <returns>the bounds at the specified location</returns>
        private Rect GetDescriptionBounds(Point position, bool isAbove, bool isLeft, double xDistance, double yDistance)
        {
            double left;
            if (isLeft)
            {
                left = position.X - xDistance - Description.Width;
            }
            else
            {
                left = position.X + xDistance;
            }
            double top;
            if (isAbove)
            {
                top = position.Y - yDistance - Description.Height;
            }
            else
            {
                top = position.Y + yDistance;
            }
            return new Rect(left, top, Description.Width, Description.Height);
        }
        #endregion Touch_Handlers_N_Helpers

        #region Visualization_Functions
        /// <summary>
        /// Filter data based on criteria
        /// </summary>
        /// <param name="criteria">Compound variable that consists of questionId and answerIDs used for filtering</param>
        private void VizOperationFilter(Criteria criteria)
        {
            bool isMatch = true;
            Submission tempSubmission;
            int numPoints = dataPointEllipses.Length;
            for (int i = 0; i < numPoints; i++)
            {
                int sIndex = int.Parse(dataPointEllipses[i].Uid);
                tempSubmission = submissionData.Submissions[sIndex];
                int numResponses = tempSubmission.Responses.Length;
                for (int j = 0; j < numResponses; j++)
                {
                    if (criteria.QuesIdAnsIdsMap.ContainsKey(tempSubmission.Responses[j].QuestionId)) // check only if the criteria includes this questionId else ignore
                    {
                        isMatch = criteria.Check(tempSubmission.Responses[j].QuestionId, tempSubmission.Responses[j].AnswerId);

                        if (!isMatch)
                        {
                            dataPointEllipses[i].Visibility = System.Windows.Visibility.Hidden;
                            if (taggedEllipses.ContainsKey("tag_" + i)) { taggedEllipses["tag_" + i].Visibility = System.Windows.Visibility.Hidden; }

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reset all filters that have been applied to allow starting again.
        /// </summary>
        private void VizOperationReset()
        {
            int numPoints = dataPointEllipses.Length;
            for (int i = 0; i < numPoints; i++)
            {
                //Make all the points visible again
                dataPointEllipses[i].Visibility = System.Windows.Visibility.Visible;

                //Make the tags visible again
                if (taggedEllipses.ContainsKey("tag_" + i)) { taggedEllipses["tag_" + i].Visibility = System.Windows.Visibility.Visible; }
            }
        }
        #endregion Visualization_Functions

        #region Tangible_Functions
        /// <summary>
        /// This is called when a tangible is put down on the table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event object containing information about tagvisualization</param>
        private void OnTangibleAdded(object sender, TagVisualizerEventArgs e)
        {
            TangibleVisualization tangibleViz = (TangibleVisualization)e.TagVisualization;
            double tOrientation = tangibleViz.Orientation, tX, tY;
            tX = tangibleViz.Center.X;
            tY = tangibleViz.Center.Y;
            VizOperationReset();
            switch (tangibleViz.VisualizedTag.Value)
            {
                case 222: //Viewpoint Republican
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("222").Name;
                    tangibleViz.myArrow.Stroke = viewpointColors[6];
                    tangibleViz.myEllipse.Stroke = viewpointColors[6];
                    //TODO could replace "49" with the question corresponding to viewpoint tangible and the answer ids to the ids in facet[0]
                    filterCriteria.AddIds("49", "6,7");
                    break;
                case 210: //Viewpoint Independent
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("210").Name;
                    tangibleViz.myArrow.Stroke = viewpointColors[3];
                    tangibleViz.myEllipse.Stroke = viewpointColors[3];
                    filterCriteria.AddIds("49", "3,4,5");
                    break;
                case 212: //Viewpoint Democrat
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("212").Name;
                    tangibleViz.myArrow.Stroke = viewpointColors[0];
                    tangibleViz.myEllipse.Stroke = viewpointColors[0];
                    filterCriteria.AddIds("49", "1,2");
                    break;
                case 208: //Question Changer
                    tangibleViz.TangibleInfo.Content = "Q?";
                    tangibleViz.myArrow.Stroke = Brushes.Green;
                    tangibleViz.myEllipse.Stroke = Brushes.Green;
                    filterCriteria.AddIds(CurrentQuestion.Uid, "All Answers");
                    break;
                case 213: //Answer Changer
                    tangibleViz.TangibleInfo.Content = "Ans?";
                    tangibleViz.myArrow.Stroke = Brushes.MediumPurple;
                    tangibleViz.myEllipse.Stroke = Brushes.MediumPurple;
                    break;
                case 214: //XAxisStarter
                    tangibleViz.TangibleInfo.Content = "Xmin";
                    tangibleViz.myArrow.Stroke = SurfaceColors.Accent3Brush;
                    tangibleViz.myEllipse.Stroke = SurfaceColors.Accent3Brush;
                    if (tX <= xRangeStart)
                    {
                        xDomainStart = xRangeStart;
                    }
                    else 
                    {
                        xDomainStart = tX;
                    }
                    UpdateXDomainLength();
                    break;
                case 209: //XAxisEnd
                    tangibleViz.TangibleInfo.Content = "Xmax";
                    tangibleViz.myArrow.Stroke = SurfaceColors.Accent3Brush;
                    tangibleViz.myEllipse.Stroke = SurfaceColors.Accent3Brush;
                    break;
                case 211: //YAxisStarter
                    tangibleViz.TangibleInfo.Content = "Ymin";
                    tangibleViz.myArrow.Stroke = SurfaceColors.ControlAccentBrush;
                    tangibleViz.myEllipse.Stroke = SurfaceColors.ControlAccentBrush;
                    break;
                case 215: //YAxisEnd
                    tangibleViz.TangibleInfo.Content = "Ymax";
                    tangibleViz.myArrow.Stroke = SurfaceColors.ControlForegroundBrush;
                    tangibleViz.myEllipse.Stroke = SurfaceColors.ControlForegroundBrush;
                    break;
                case 223: //Tagger
                    tangibleViz.TangibleInfo.Content = "Tagger";
                    tangibleViz.myArrow.Stroke = Brushes.PeachPuff;
                    tangibleViz.myEllipse.Stroke = Brushes.PeachPuff;
                    allowTagging = true;
                    break;
            }
            LogMsg(filterCriteria.ToLogString());
            VizOperationFilter(filterCriteria);
        }

        /// <summary>
        /// This is called when a tangible is moved (rotation, translation) on the table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event object containing information about tagvisualization</param>
        private void OnTangibleMoved(object sender, TagVisualizerEventArgs e)
        {
            TangibleVisualization tangibleViz = (TangibleVisualization)e.TagVisualization;
            double tOrientation = tangibleViz.Orientation, tX, tY;
            tX = tangibleViz.Center.X;
            tY = tangibleViz.Center.Y;
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = tOrientation;
            //tOrientation = (tOrientation == 0.0) ? 360.00 : tOrientation;
            //if (tOrientation < 0)
            //{
            //    tOrientation += 360.00;  
            //}

            

            VizOperationReset();
            switch (tangibleViz.VisualizedTag.Value)
            {
                case 222: //Viewpoint Republican
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("222").Name;

                    if (tOrientation >= 0 && tOrientation <= 360) //these conditions required because sometimes the orientation value becomes negative or is more than 360
                    {
                        int numFacets = configData.FindTangibleFromId("222").Rotation.Length;
                        int facetIndex = (int)Math.Floor(tOrientation / (360.00 / numFacets));
                        string facetAnswerIds = configData.FindTangibleFromId("222").Rotation[facetIndex].AnswerIds;
                        if (prevRepViewPtFacetIndex != facetIndex)
                        {
                            filterCriteria.RemoveIds("49", "6,7");
                            filterCriteria.AddIds("49", facetAnswerIds);
                            prevRepViewPtFacetIndex = facetIndex;
                        }
                    }
                    break;
                case 210: //Viewpoint Independent
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("210").Name;

                    if (tOrientation >= 0 && tOrientation <= 360) //these conditions required because sometimes the orientation value becomes negative or is more than 360
                    {
                        int numFacets = configData.FindTangibleFromId("210").Rotation.Length;
                        int facetIndex = (int)Math.Floor(tOrientation / (360.00 / numFacets));
                        string facetAnswerIds = configData.FindTangibleFromId("210").Rotation[facetIndex].AnswerIds;
                        if (prevIndViewPtFacetIndex != facetIndex)
                        {
                            filterCriteria.RemoveIds("49", "3,4,5");
                            filterCriteria.AddIds("49", facetAnswerIds);
                            prevIndViewPtFacetIndex = facetIndex;
                        }
                    }
                    break;
                case 212: //Viewpoint Democrat
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("212").Name;
                    if (tOrientation >= 0 && tOrientation <= 360) //these conditions required because sometimes the orientation value becomes negative or is more than 360
                    {
                        int numFacets = configData.FindTangibleFromId("212").Rotation.Length;
                        int facetIndex = (int)Math.Floor(tOrientation / (360.00 / numFacets));
                        string facetAnswerIds = configData.FindTangibleFromId("212").Rotation[facetIndex].AnswerIds;
                        if (prevDemViewPtFacetIndex != facetIndex)
                        {
                            filterCriteria.RemoveIds("49", "1,2");
                            filterCriteria.AddIds("49", facetAnswerIds);
                            prevDemViewPtFacetIndex = facetIndex;
                        }
                    }
                    break;
                case 208: //Question Changer
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("208").Name;
                    if (tOrientation >= 0 && tOrientation <= 360) //these conditions required because sometimes the orientation value becomes negative or is more than 360
                    {
                        int numFacets = configData.FindTangibleFromId("208").Rotation.Length;
                        int facetIndex = (int)Math.Floor(tOrientation / (360.00 / numFacets));
                        string facetQuestionId = configData.FindTangibleFromId("208").Rotation[facetIndex].QuestionId;
                        string facetAnswerIds = configData.FindTangibleFromId("208").Rotation[facetIndex].AnswerIds;
                        if (prevQuesChangerFacetIndex != facetIndex)
                        {
                            //Remove the left over answerIds from other questions
                            //FIX for the bug where when question and answer tangible are rotated together then there are leftover answerIds in previous question
                            filterCriteria.AddIds(CurrentQuestion.Uid, "All Answers");
                            filterCriteria.AddIds(facetQuestionId, facetAnswerIds);
                            SetQuestion(facetQuestionId);
                            prevQuesChangerFacetIndex = facetIndex;
                        }
                    }
                    break;
                case 213: //Answer Changer
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("213").Name;
                    if (tOrientation >= 0 && tOrientation <= 360) //these conditions required because sometimes the orientation value becomes negative or is more than 360
                    {
                        //In case of answer changer tangible the Facet list has been left blank because it depends on the question id
                        int numFacets = configData.FindQuestionFromId(CurrentQuestion.Uid).Answers.Length + 1; //+1 for adding the case in which all answers appear
                        int facetIndex = (int)Math.Floor(tOrientation / (360.00 / numFacets));
                        if (prevAnsChangerFacetIndex != facetIndex)
                        {
                            if (facetIndex == 0) // index 0 corresponds to all answers
                            {
                                SetAnswer(CurrentQuestion.Uid, "");
                                filterCriteria.AddIds(CurrentQuestion.Uid, "All Answers");
                            }
                            else
                            {
                                string ansId = configData.FindQuestionFromId(CurrentQuestion.Uid).Answers[facetIndex - 1].AnswerId;
                                //Since we know that at a time only one answer can appear for answer changer tangible we will manually replace the value of first element in criteria
                                filterCriteria.QuesIdAnsIdsMap[CurrentQuestion.Uid][0] = ansId;
                                SetAnswer(CurrentQuestion.Uid, ansId);
                            }
                            prevAnsChangerFacetIndex = facetIndex;
                        }

                    }
                    break;
                case 214: //XAxisStarter
                    tangibleViz.TangibleInfo.Content = configData.FindTangibleFromId("214").Name;
                    if (tOrientation >= 0 && tOrientation <= 360) //these conditions required because sometimes the orientation value becomes negative or is more than 360
                    {
                        int numFacets = configData.FindTangibleFromId("214").Rotation.Length;
                        int facetIndex = (int)Math.Floor(tOrientation / (360.00 / numFacets));
                        //TODO Throws exception every now and then
                        string facetQuestionId = configData.FindTangibleFromId("214").Rotation[facetIndex].QuestionId;
                        string facetAnswerIds = configData.FindTangibleFromId("214").Rotation[facetIndex].AnswerIds;

                        if (prevXMinFacetIndex != facetIndex)
                        {
                            SetXAxis(facetQuestionId);
                            redrawPointsOnAxisChange = true;
                            DrawXTicks();
                            DrawPoints(1, 1);
                            prevXMinFacetIndex = facetIndex;
                        }
                    }
                    prevXDomainStart = xDomainStart;
                    if (tX <= xRangeStart)
                    {
                        xDomainStart = xRangeStart;
                    }
                    else 
                    {
                        xDomainStart = tX;
                    }

                    if (Math.Abs(prevXDomainStart - xDomainStart) >= axisChangeTolerance)
                    {
                        redrawPointsOnXAxisZoom = true;
                        DrawPoints(1, 1);
                    }
                    //UpdateXDomainLength();
                    break;
                case 209: //XAxisEnd
                    prevXDomainEnd = xDomainEnd;
                    if (tX >= xRangeEnd)
                    {
                        xDomainEnd = xRangeEnd;
                    }
                    else
                    {
                        xDomainEnd = tX;
                    }
                    if (Math.Abs(prevXDomainEnd - xDomainEnd) >= axisChangeTolerance)
                    {
                        redrawPointsOnXAxisZoom = true;
                        DrawPoints(1, 1);
                    }
                    break;
                case 211: //YAxisStarter
                    if (tOrientation >= 0 && tOrientation <= 360) //these conditions required because sometimes the orientation value becomes negative or is more than 360
                    {
                        int numFacets = configData.FindTangibleFromId("211").Rotation.Length;
                        int facetIndex = (int)Math.Floor(tOrientation / (360.00 / numFacets));
                        string facetQuestionId = configData.FindTangibleFromId("211").Rotation[facetIndex].QuestionId;
                        string facetAnswerIds = configData.FindTangibleFromId("211").Rotation[facetIndex].AnswerIds;

                        if (prevYMinFacetIndex != facetIndex)
                        {
                            SetYAxis(facetQuestionId);
                            redrawPointsOnAxisChange = true;
                            DrawYTicks();
                            DrawPoints(1, 1);
                            prevYMinFacetIndex = facetIndex;
                        }
                    }
                    break;
                case 215: //YAxisEnd
                    break;
                case 223: //Tagger
                    break;
            }

            //LogMsg(filterCriteria.ToLogString());
            VizOperationFilter(filterCriteria);
        }

        /// <summary>
        /// This is called when a tangible is removed from the table. 
        /// Note: This event is called only after the "LostTagTimeout" has completed, so expect delays, 
        /// but be thankful otherwise things will start flickering due to instability of detection of a marker.   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event object containing information about tagvisualization</param>
        private void OnTangibleRemoved(object sender, TagVisualizerEventArgs e)
        {
            VizOperationReset();
            TangibleVisualization tangibleViz = (TangibleVisualization)e.TagVisualization;
            switch (tangibleViz.VisualizedTag.Value)
            {
                case 222: //Viewpoint Republican
                    filterCriteria.RemoveIds("49", "6,7");
                    break;
                case 210: //Viewpoint Independent
                    filterCriteria.RemoveIds("49", "3,4,5");
                    break;
                case 212: //Viewpoint Democrat
                    filterCriteria.RemoveIds("49", "1,2");
                    break;
                case 208: //Question Changer
                    //Do nothing as the question that was set has to persist
                    break;
                case 213: //Answer Changer
                    //Once the answer changer is taken off the table, filter should be removed
                    filterCriteria.AddIds(CurrentQuestion.Uid, "All Answers"); //same as removing previously set ids
                    SetAnswer(CurrentQuestion.Uid, "");
                    break;
                case 214: //XAxisStarter
                    prevXDomainStart = xDomainStart;
                    xDomainStart = xRangeStart;
                    redrawPointsOnXAxisZoom = true;
                    DrawPoints(1, 1);
                    break;
                case 209: //XAxisEnd
                    prevXDomainEnd = xDomainEnd;
                    xDomainEnd = xRangeEnd;
                    redrawPointsOnXAxisZoom = true;
                    DrawPoints(1, 1);
                    break;
                case 211: //YAxisStarter
                    break;
                case 215: //YAxisEnd
                    break;
                case 223: //Tagger
                    allowTagging = false;

                    //Remove the ellipses from MainCanvas and clear the taggedEllipses list
                    foreach (string tagKey in taggedEllipses.Keys)
                    {
                        MainCanvas.Children.Remove(taggedEllipses[tagKey]);
                    }
                    taggedEllipses.Clear();
                    break;
            }
            LogMsg(filterCriteria.ToLogString());
            VizOperationFilter(filterCriteria);
        }

        #endregion Tangible_Functions

        #region System_Window_Event_Handlers
        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
        #endregion System_Window_Event_Handlers
    }
}
