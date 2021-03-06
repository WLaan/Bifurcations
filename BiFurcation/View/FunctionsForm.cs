﻿using System;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

using System.Windows.Forms;

namespace BiFurcation {

  public partial class FunctionsForm : Form, IFunctionsView {

    #region fields
    private CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
    private Control4AllViews control4AllViews;

    private int mouseX = 0;
    private int mouseY = 0;
    private bool m_DrawingBox;
    #endregion

    public FunctionsForm() {

      InitializeComponent();

      control4AllViews = new Control4AllViews(this);

      control4FunctionsView = new Control4FunctionsView(this, control4AllViews);
      control4AllViews.control4FunctionsView = control4FunctionsView;

      DiagramsForm diagram = DiagramsForm.GetInvisible;
      control4FunctionsView.Control4DiagramView = 
        new Control4DiagramView(diagram, control4AllViews);
      control4AllViews.Control4DiagramView = control4FunctionsView.Control4DiagramView;

      NonLineairSystemsForm m = NonLineairSystemsForm.GetInvisible;
      control4AllViews.Control4NonLineairSystems = m.Control4NonLineairSystems;
      DefineColorsForm defColorForm = DefineColorsForm.GetInvisible;
      defColorForm.CombinedControl = m.Control4NonLineairSystems;

      Params2Form();

      this.Cursor = Cursors.Default;
      pictureBox.Cursor = Cursors.Cross;
      listBoxXValues.DrawItem += listBox_DrawItem;

    }

    private void DoResize() {
      panel4Image.Left = 0;
      panel4Image.Top = 0; 
      panel4Image.Height = ClientSize.Height - 1;
      panel4Image.Width = ClientSize.Width - panelMainPanel.Width - 2;

      panelMainPanel.Top = 0; 
      panelMainPanel.Left = this.ClientSize.Width - panelMainPanel.Width - 1;
      panelMainPanel.Height = ClientSize.Height;// panel4Image.Width;

      panelOneDimensiolFunctions.Left = 5;
      panelOneDimensiolFunctions.Top = 5;
      panelOneDimensiolFunctions.Width = panelMainPanel.Width - 10;

      panel6.Left = 5;
      panel6.Top = panelOneDimensiolFunctions.Top + panelOneDimensiolFunctions.Height + 10;
      panel6.Width = panelMainPanel.Width - 10;
      panel6.Height = panelMainPanel.Height - panel6.Top - 10;
      listBoxXValues.Height = panel6.Height - listBoxXValues.Top - 10;
      Refresh();
    }
    private void setXYStartStop() {
      control4FunctionsView.SetXYBoarders(textBoxStartX.Text, textBoxEndX.Text, textBoxStartY.Text, textBoxEndY.Text);
    }
    private void setVisibility() {
      bool henonORmandelbrot = control4FunctionsView.CurrFunctionType == FunctionType.Henon;// ||
                              // control4FunctionsView.CurrFunctionType == FunctionType.Mandelbrot;
      labelC.Visible = control4FunctionsView.CurrFunctionType != FunctionType.FixedPolynomial;
      textBoxC.Visible = control4FunctionsView.CurrFunctionType != FunctionType.FixedPolynomial;
      radioButtonParamaterB.Enabled = control4FunctionsView.CurrFunctionType != FunctionType.FixedPolynomial;
      radioButtonParamaterC.Enabled = control4FunctionsView.CurrFunctionType != FunctionType.FixedPolynomial;
      textBoxB.Enabled = control4FunctionsView.CurrFunctionType != FunctionType.FixedPolynomial;
      textBoxC.Enabled = control4FunctionsView.CurrFunctionType != FunctionType.FixedPolynomial;
      buttonCalcHenon.Visible = henonORmandelbrot;

      labelC.Visible = !henonORmandelbrot;
      textBoxC.Visible = !henonORmandelbrot;
      labelSeed.Visible = !henonORmandelbrot;
      textBoxSeed.Visible = !henonORmandelbrot;
      if (henonORmandelbrot)
        radioButtonParamaterA.Checked = true;
      radioButtonParamaterB.Visible = !henonORmandelbrot;
      radioButtonParamaterC.Visible = !henonORmandelbrot;
      panelAttractionIterations.Enabled = !henonORmandelbrot;

      panelHenon.Visible = henonORmandelbrot;
      panelLogistic.Visible = !panelHenon.Visible;// control4FunctionsView.CurrFunctionType == FunctionType.FixedPolynomial;// || control4FunctionsView.CurrFunctionType == FunctionType.SinFunction;
      labelIterations.Visible = true;// henonORmandelbrot;
      textBoxMaxIterations.Visible = true;// henonORmandelbrot;

      buttonSimulate.Visible = !henonORmandelbrot;
      buttonSimulatePar.Visible = !henonORmandelbrot;
      buttonEmulate.Visible = !henonORmandelbrot;
      createGIFOverIterationsMenuItem.Visible = !henonORmandelbrot;
      //     createGIFOverParameterMenuItem.Visible = control4FunctionsView.CurrFunctionType != FunctionType.Henon;
    }

    #region events
    public void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
      // All unhandled exceptions will end up here, so do what you need here

      // log the error
      // show the error
      // shut the application down if needed
      // ROLLBACK database changes 
      // and so on...
    }
    private void ButtonDefineColors_Click(Object sender, EventArgs e) {
      control4AllViews.OpenColorDefView();
    }
    private void BiFurcationForm_ResizeEnd(Object sender, EventArgs e) {
      DoResize();
    }
    private void pictureBox_MouseDown(Object sender, MouseEventArgs e) {
      mouseX = e.X;
      mouseY = e.Y;
      if (e.Button == MouseButtons.Left) {
        m_DrawingBox = true;
        control4FunctionsView.MouseDown(e.X, e.Y, pictureBox.Width, pictureBox.Height);
        this.Cursor = Cursors.Default;
        pictureBox.Cursor = Cursors.Cross;
      }
    }
    private void pictureBox_MouseMove(Object sender, MouseEventArgs e) {
      PointD p = control4FunctionsView.ShowMouseCoords(e.X, e.Y, pictureBox.Width, pictureBox.Height);
      if (m_DrawingBox)
        control4FunctionsView.MouseMove(e.X, e.Y, pictureBox.Width, pictureBox.Height);
      else
        if (control4FunctionsView.SeedWithMouse) {
        control4FunctionsView.Seed = p.X;
        labelChoozenFunction.Text = control4FunctionsView.NewFunctionString;
        control4FunctionsView.PlotFunction();
      }
     
      labelCurrX.Text = p.X.ToString("0.000");
      labelCurrY.Text = p.Y.ToString("0.000");
    }
    private void pictureBox_MouseUp(Object sender, MouseEventArgs e) {
      if (m_DrawingBox) {
        m_DrawingBox = false;
        control4FunctionsView.MouseUp(e.X, e.Y, pictureBox.Width, pictureBox.Height);

        this.Cursor = Cursors.WaitCursor;
        Application.DoEvents();
        this.Cursor = Cursors.Default;
        pictureBox.Cursor = Cursors.Cross;
      }
    }
    private void buttonReset_Click(Object sender, EventArgs e) {
      control4FunctionsView.Reset();
    }
    private void imageToEditorMenuItem_Click(Object sender, EventArgs e) {
      if (pictureBox.Image != null)
        control4AllViews.StartImageEditor(pictureBox.Image);
    }
    private void comboBoxChoozenFunction_SelectedIndexChanged(Object sender, EventArgs e) {
      control4FunctionsView.CurrFunctionType = (FunctionType)comboBoxChoozenFunction.SelectedIndex; // FunctionType.FixedPolynomial;
      setVisibility();
      Params2Form();
    }
    private void buttonEmulate_Click(Object sender, EventArgs e) {
      control4FunctionsView.SetPoints();
    }
    private void textBoxStopParValue_TextChanged(Object sender, EventArgs e) {
      control4FunctionsView.DiagramStopParameter = control4AllViews.Text2Float(textBoxStopParValue.Text);
    }
    private void listBoxFeigenbaum_SelectedIndexChanged(Object sender, EventArgs e) {
      textBoxA.Text = listBoxFeigenbaum.SelectedItem.ToString();
      if (checkBoxSetSeed.Checked) {
        decimal s = control4AllViews.Text2Float(textBoxA.Text);
        s = 1 - 1 / s;
        textBoxSeed.Text = s.ToString();
      }
      control4FunctionsView.SetPoints();
    }
    private void textBoxA_TextChanged(Object sender, EventArgs e) {
      control4FunctionsView.A = control4AllViews.Text2Float(textBoxA.Text);
      if (control4FunctionsView.CurrFunctionType == FunctionType.FixedPolynomial)
        textBoxB.Text = (-control4FunctionsView.A).ToString();
      labelChoozenFunction.Text = control4FunctionsView.CurrentFunction.FunctionStr;
    }
    private void textBoxB_TextChanged(Object sender, EventArgs e) {
      control4FunctionsView.B = control4AllViews.Text2Float(textBoxB.Text);
      labelChoozenFunction.Text = control4FunctionsView.CurrentFunction.FunctionStr;
    }
    private void textBoxC_TextChanged(Object sender, EventArgs e) {
      control4FunctionsView.C = control4AllViews.Text2Float(textBoxC.Text);
      labelChoozenFunction.Text = control4FunctionsView.CurrentFunction.FunctionStr;
    }
    private void textBoxSeed_TextChanged(Object sender, EventArgs e) {
      control4FunctionsView.Seed = (decimal)control4AllViews.Text2Float(textBoxSeed.Text);
    }
    private void textBoxMaxIterations_TextChanged(Object sender, EventArgs e) {
      int m = control4FunctionsView.MaxFunctionIterations;
      Int32.TryParse(textBoxMaxIterations.Text, out m);
      control4FunctionsView.MaxFunctionIterations = m;
    }
    private void radioButtonParamater_CheckedChanged(Object sender, EventArgs e) {
      control4FunctionsView.SetDiagramParameters(radioButtonParamaterA.Checked, radioButtonParamaterB.Checked, radioButtonParamaterC.Checked);
      if (radioButtonParamaterC.Checked)
        textBoxStopParValue.Top = radioButtonParamaterC.Top;
      else
        if (radioButtonParamaterB.Checked)
        textBoxStopParValue.Top = radioButtonParamaterB.Top;
      else
        textBoxStopParValue.Top = radioButtonParamaterA.Top;
      textBoxStopParValue.Top -= (textBoxStopParValue.Height - radioButtonParamaterA.Height) / 2;
      labelChoozenFunction.Text = control4FunctionsView.NewFunctionString;
    }
    private void textBox_KeyDown(Object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        labelChoozenFunction.Text = control4FunctionsView.NewFunctionString;
        control4FunctionsView.SetPoints();
      }
    }
    private void buttonSimulate_Click(Object sender, EventArgs e) {
      control4FunctionsView.Simulate(false);
    }
    private void buttonSimulatePar_Click(Object sender, EventArgs e) {
      control4FunctionsView.SimulatePar(false);
    }
    private void textBoxStartX_KeyDown(Object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter)
        setXYStartStop();
    }
    private void textBoxEndX_KeyDown(Object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter)
        setXYStartStop();
    }
    private void textBoxStartY_KeyDown(Object sender, KeyEventArgs e) {
      if (e.KeyCode== Keys.Enter)
        setXYStartStop();
    }
    private void textBoxEndY_KeyDown(Object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter)
        setXYStartStop();
    }
    private void checkBoxHideFurcationLines_CheckedChanged(Object sender, EventArgs e) {
      control4FunctionsView.DrawFurcations = !checkBoxHideFurcationLines.Checked;
    }
    private void textBoxGIFFilename_TextChanged(Object sender, EventArgs e) {
      control4FunctionsView.FunctionGifFileName = textBoxGIFFilename.Text;
    }
    private void createGIFOverIterationsMenuItem_Click(Object sender, EventArgs e) {
      control4FunctionsView.Simulate(true);
    }
    private void createGIFOverParameterMenuItem_Click(Object sender, EventArgs e) {
      control4FunctionsView.SimulatePar(true);
    }
    private void calcTrajectoryAtCursorMenuItem_Click(Object sender, EventArgs e) {
      control4FunctionsView.InitDrawTrajectory(mouseX, mouseY, pictureBox.Width, pictureBox.Height);
      buttonNextTrajectory.Enabled = true;
      labelNoPoints.Text = control4FunctionsView.NoPoints.ToString();
    }
    private void buttonNextTrajectory_Click(Object sender, EventArgs e) {
      numericUpDownMultPointsPerStep.Enabled = false;
      buttonNextTrajectory.Enabled = false;
      control4FunctionsView.NextTrajectory();
      numericUpDownMultPointsPerStep.Enabled = true;
      buttonNextTrajectory.Enabled = true;
      labelNoPoints.Text = control4FunctionsView.NoPoints.ToString();
    }
    private void buttonStopTrajectory_Click(Object sender, EventArgs e) {
      control4FunctionsView.StopTrajectory();
      buttonNextTrajectory.Enabled = false;
    }
    private void numericUpDownDotSize_ValueChanged(Object sender, EventArgs e) {
      control4FunctionsView.SetHenonDotsize(numericUpDownDotSize.Value.ToString());
    }
    private void buttonCalcHenon_Click(Object sender, EventArgs e) {
      control4FunctionsView.SetPoints();
      setVisibility();
    }
    private void checkBoxOmitFirstIterations_CheckedChanged(Object sender, EventArgs e) {
      control4FunctionsView.OmitFirst = checkBoxOmitFirstIterations.Checked;
    }
    private void buttonBifurcationDiagram_Click(Object sender, EventArgs e) {
      DiagramsForm diagram = DiagramsForm.Instance;
      diagram.Control4FunctionsView = control4FunctionsView;
      diagram.Show();
      diagram.BringToFront();
      control4FunctionsView.SetDiagram(diagram);//, radioButtonParamaterA.Checked, radioButtonParamaterB.Checked, radioButtonParamaterC.Checked);
      diagram.Params2Form();
    }
    private void buttonCombinations_Click(Object sender, EventArgs e) {
      control4AllViews.OpenSystem2Form();
    }
    private void buttonCancelGif_Click(Object sender, EventArgs e) {
      control4FunctionsView.StopThread();
    }
    private void numericUpDownInitialPoints_ValueChanged(Object sender, EventArgs e) {
      control4FunctionsView.InitionalDots= (int)numericUpDownInitialPoints.Value;
    }
    private void numericUpDownMultPointsPerStep_ValueChanged(Object sender, EventArgs e) {
      control4FunctionsView.MultStepPoints = (int)numericUpDownMultPointsPerStep.Value;
    }
    private void checkBoxSeedWithMouseCursor_CheckedChanged(Object sender, EventArgs e) {
      control4FunctionsView.SeedWithMouse = checkBoxSeedWithMouseCursor.Checked;
      if (!control4FunctionsView.SeedWithMouse) {
        control4FunctionsView.Seed = (decimal)control4AllViews.Text2Float(textBoxSeed.Text);
        control4FunctionsView.PlotFunction();
      }
    }
    private void listBox_DrawItem(object sender, DrawItemEventArgs e) {
      e.DrawBackground();
      if (e.Index < 0) return;
      Graphics g = e.Graphics;
      g.FillRectangle(new SolidBrush(Color.White), e.Bounds);
      ListBox lb = (ListBox)sender;
      if (e.Index < lb.Items.Count - control4FunctionsView.CurrentFunction.setCount)
        g.DrawString(lb.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Black), new PointF(e.Bounds.X, e.Bounds.Y));
      else
        g.DrawString(lb.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Red), new PointF(e.Bounds.X, e.Bounds.Y));

      e.DrawFocusRectangle();
    }
    private void checkBoxShowFn_CheckedChanged(Object sender, EventArgs e) {
      for (int n = 1; n < 10; n++)
        control4FunctionsView.SetFInclude(n, false);
      control4FunctionsView.SetFInclude(control4FunctionsView.CurrentFunction.setCount, checkBoxShowFn.Checked);
    }
    #endregion

    #region interface
    private void Setimage(Image im) {
      try {
        if (this.InvokeRequired) {
          this.EndInvoke(this.BeginInvoke(new MethodInvoker(delegate {
            this.Setimage(im);
          })));
        }
        else {
          pictureBox.Image = im;
        }
      }
      catch { }
    }
    private Bitmap formImage;
    public Bitmap FormImage {
      set {
        formImage = value;
        Setimage(value);
      }
      get {
        return formImage;
      }
    }

    private Control4FunctionsView control4FunctionsView;
    public Control4FunctionsView Control4FunctionsView {
      set {
        control4FunctionsView = value;
      }
    }

    public void FillXValues(List<decimal> points) {
      if (points.Count == 0) return;
      try {
        if (this.InvokeRequired) {
          this.EndInvoke(this.BeginInvoke(new MethodInvoker(delegate {
            this.FillXValues(points);
          })));
        }
        else {
          int start = 0;
          if (points.Count > 1000) {
            start = points.Count - 1000;
            labelLast1000.Text = "Last 1000";
          }
          else {
            labelLast1000.Text = "";
          }
          labelSetCount.Text = control4FunctionsView.CurrentFunction.setCount.ToString();
          listBoxXValues.Items.Clear();
          for (int p = start; p < points.Count; p++)
            listBoxXValues.Items.Add(points[p]);
          listBoxXValues.SelectedIndex = points.Count - 1;
          //   checkBoxShowFn.Visible = control4FunctionsView.CurrentFunction.setCount > 0;
          if (control4FunctionsView.CurrentFunction.setCount == 0)
            checkBoxOmitFirstIterations.Checked = false;
          checkBoxOmitFirstIterations.Visible = control4FunctionsView.CurrentFunction.setCount > 0;

          checkBoxShowFn.Text = "Show F" + control4FunctionsView.CurrentFunction.setCount;
          control4FunctionsView.SetFInclude(0, true);
          if (control4FunctionsView.CurrentFunction.setCount > 0 || !checkBoxShowFn.Checked)
            for (int n = 1; n < 10; n++)
              control4FunctionsView.SetFInclude(n, false);
          if (checkBoxShowFn.Checked) {
            if (control4FunctionsView.CurrentFunction.setCount > 0)
              control4FunctionsView.SetFInclude(control4FunctionsView.CurrentFunction.setCount, true);
          }
        }
      }
      catch { } 
    }
    public void SetEnabled(bool e) {
      try {
        if (this.InvokeRequired) {
          this.EndInvoke(this.BeginInvoke(new MethodInvoker(delegate {
            this.SetEnabled(e);
          })));
        }
        else {
          pictureBox.Enabled = e;
          panel4.Enabled = e;
          panel6.Enabled = e;
          panel7.Enabled = e;
          panelAttractionIterations.Enabled = e;
        }
      }
      catch { }
    }
    public void Params2Form() {
      switch (control4FunctionsView.CurrFunctionType) {
        case FunctionType.FixedPolynomial:
        case FunctionType.Henon:
          if (control4FunctionsView.CurrFunctionType != FunctionType.Henon)
            textBoxB.Text = (-control4FunctionsView.A).ToString();
          textBoxC.Text = "0";
          if (control4FunctionsView.CurrFunctionType == FunctionType.Henon)
            checkBoxHideFurcationLines.Checked = true;
          radioButtonParamaterA.Checked = true;
          break;
      }
      textBoxA.Text = control4FunctionsView.A.ToString();
      textBoxB.Text = control4FunctionsView.B.ToString();
      textBoxC.Text = control4FunctionsView.C.ToString();
      textBoxSeed.Text = control4FunctionsView.Seed.ToString();
      labelChoozenFunction.Text = control4FunctionsView.NewFunctionString;
      textBoxStopParValue.Text = control4FunctionsView.DiagramStopParameter.ToString("0.000");

      textBoxStartX.Text = control4FunctionsView.Xmin.ToString();
      textBoxEndX.Text = control4FunctionsView.Xmax.ToString();
      textBoxStartY.Text = control4FunctionsView.Ymin.ToString();
      textBoxEndY.Text = control4FunctionsView.Ymax.ToString();
      textBoxMaxIterations.Text = control4FunctionsView.MaxFunctionIterations.ToString();

      comboBoxChoozenFunction.SelectedIndex = (int)control4FunctionsView.CurrFunctionType;
      switch (control4FunctionsView.ParNum) {
        case 0:
          radioButtonParamaterC.Checked = true;
          break;
        case 1:
          radioButtonParamaterB.Checked = true;
          break;
        case 2:
          radioButtonParamaterA.Checked = true;
          break;
      }
      setVisibility();
      control4FunctionsView.SetPoints();
    }

    #region IProgressbar
    public void SetProgressBar(int max) {
      if (this.InvokeRequired) {
        this.EndInvoke(this.BeginInvoke(new MethodInvoker(delegate {
          this.SetProgressBar(max);
        })));
      }
      else {
        progressBar.Value = 0;
        progressBar.Maximum = max;
        progressBar.Visible = true;
        progressBar.BringToFront();
      }
    }
    public void EndGenerate() {
      if (this.InvokeRequired) {
        this.EndInvoke(this.BeginInvoke(new MethodInvoker(delegate {
          this.EndGenerate();
        })));
      }
      else {
        progressBar.Visible = false;
      }
    }
    public int GIFProgress {
      set {
       // labelGIFProgress.Text = value.ToString();
      }
    }
    public void ReportProgress(object sender, ProgressReportModel e) {
      if (e.PercentageComplete < progressBar.Maximum)
      progressBar.Value = e.PercentageComplete;
    }
    #endregion

    #endregion

  }

}