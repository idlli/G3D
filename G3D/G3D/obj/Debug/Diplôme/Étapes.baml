   M S B A M L   `   `   `  ����>  :G3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:G3DG3D  5      d `DotNetProjects.Input.Toolkit, Version=6.1.94.0, Culture=neutral, PublicKeyToken=79778c5f2eed289bgKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.ToolkitSystem.Windows.Controls 6.       G3D.Diplôme.Étapes   5      ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      _ [MaterialDesignThemes.Wpf, Version=4.5.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      localclr-namespace:G3D  5      ZcontrolsKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.Toolkit  6.   $��Regular"�5	      $��14(�5
      ��5      :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension  .��5      �� ���#f3f4fb  �� CornerRadius$  5��6"   ._�5   
   �  � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� 
�������$  5��6(    �� Padding$ 30,20q�69   ._�5      � .+�5      �� $��Center=�6    $��12(�6;   ���#57606fRechercher par6n   �� 5      -   5        $��SearchBy��5   �   -  'MaterialDesignThemes.Wpf.ComboBoxAssist   ShowSelectedItem .  $��	10,1,10,0q�6�   $��13(�6�   $��Medium"�6�   ���#57606f���#57606f.@�5      ��  �� 
IsSelected . $��	Stagiaire��69   �� 5      $��Groupe��6'   �� 5      $��Filière��6'   5      -  'System.Windows.Controls.AutoCompleteBox  5       -   5          Name$ SearchByBox��5    \   )  #MaterialDesignThemes.Wpf.HintAssist   FloatingScale$ .9Q�6/   $	��250��6q   $
��25,0q�6�      AutoCompleteBoxStyle#���   6�   {�.    IsTextCompletionEnabled . 	   
FilterMode$	 Contains=�6C  $��12,10q�6Y  
   Hint$
 Rechercher ...��6i  $��Center=�6�  �� 5#      -   5         ��Name$ SelectedTap��5%      $ .9Q�5$      ���#57606f{�.  $��1q�5)      ���#a4b0be$	��230��5+      $��12,11q�5,       " MaterialDesignOutlinedComboBox#��[ 5-      $#
 Statut suivant des stagiaires��5.         
Foreground �#a4b0be$��12(�50      .@�51      �� .��6'   Edité6.   �� 52      .��6'   	Envoyé6/   �� 53      .��6'   Signé6.    � 	Resources 54        MaterialDesignPaper �� Color��%)   &	       �� 55      $ White��6J   56      57      58      59      � 5:      -   5         �Name$ ListItemsPanel��5:   4   $��1��6   $��2��6#   $	��420��6L   $y�	Collapsed=�6[   $��25,0,0,0q�6r    5;      �� 5<      $��* �6$   �� 5=      $
��Auto �6$   .+�5?      �� 
�������$  5��6,   $ 30,20,30,30q�6=   ._�5@      &   System.Windows.Controls.DataGrid  -   5        $��ListData��5Q      $��1��5B      $��12(�5C      "   VerticalScrollBarVisibility$
 Auto=�5D      ���#dfe4ea   CanUserAddRows .     AutoGenerateColumns .     HeadersVisibility$	 All=�5H      
�������-  'MaterialDesignThemes.Wpf.DataGridAssist   CellPadding$ 12q�5J         CornerRadius$ 0��5K         ColumnHeaderPadding$ 16q�5L         CanUserResizeColumns .     CanUserDeleteRows .     CanUserResizeRows .  ��5A      �� *	ListItems+ 5R      'clr-namespace:G3D.Domain
G3D.Domain  5T         G3D.Domain.BindingProxy  DataContextProxy   Data��5R      %Z   &	         5S      %domainclr-namespace:G3D.Domain  5T       6   �� 6   5U         Columns 5V      4  .System.Windows.Controls.DataGridCheckBoxColumn  5W       - )MaterialDesignDataGridCheckBoxColumnStyle1	  +System.Windows.Controls.DataGridBoundColumn 	  ElementStyle# [ 5Z       4 0MaterialDesignDataGridCheckBoxColumnEditingStyle 	  EditingElementStyle# [ 5[       	  Binding 5Y      �� *
IsSelected+ �� UpdateSourceTrigger$ PropertyChanged=�,
  &System.Windows.Controls.DataGridColumn  
  Header  5]   "   �� 5^   &   
������ ._�5_   *   �� -   5        ! ��Name$! GridCheckBoxColumn��5_   3   " d� 	IsChecked" 6O   �� *Data.IsAllListItemsSelected+# �� Source## [ 5`   '   $ 
  HeaderStyle$ 5b   "   �� 5c   &   % �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"%  6,    & "MaterialDesignDataGridColumnHeader& �� BasedOn#& [ 6W   .�5d   *   �� ' �� Property' � ��( �� Value$( Center=�6W   5e   '   5g      0  *System.Windows.Controls.DataGridTextColumn  5h      $	  CEF��5j      ) 
  
IsReadOnly) .  ) %MaterialDesignDataGridTextColumnStyle# [ 5l       5i      �� *Cef+5l        5m      $  NOM ET PRÉNOM��5o      ) . # [ 5q       5n      �� *
NomPrénom+5q      5s      5t      �� 5u      $��1��6   
�������$  5��69   $ 30,20q�6J   $��0,25,0,0q�6Z   ._�5v      � .+�5w      �� $��Center=�6$   $
��Left=�6?   ���#57606f$��12(�6o   Stagiaires : �� 6�   -   5        * �Name$* SelectedCount��5w   �   $��Medium"�6�     PrimaryHueMidBrush#��[ 6�   .�6�   06�   6�   �� 5�      -   5        $��SupprimerStagiaires��5�   N   $��Right=�5�   !    !	 MaterialDesignFlatLightButton#��[	 5�      + � ToolTip$+ Supprimer des stagiaire��5�      $��34��5�   !   ���#eb3b5a
������ 3  -MaterialDesignThemes.Wpf.ButtonProgressAssist,   IndicatorForeground
, �   �-   Opacity$- 0Q�5�   !   1  +MaterialDesignThemes.Wpf.ToggleButtonAssist.   SwitchTrackOffBackground$. Black��5�   !   /   SwitchTrackOnBackground$/ Black��5�   !   $��0q�5�   !   +  %MaterialDesignThemes.Wpf.ButtonAssist0   CornerRadius$0 5��5�   !   $��34��5�      .��5�      '  !MaterialDesignThemes.Wpf.PackIcon  1   Kind$1 DeleteOutline=�5�      $��20��5�      $��20��5�      5�      5�      5�      5�      � 5�      $��1��6   $��0,25,0,0q�6    $��2��62    5�      �� 5�      $
��Auto �6'   �� 5�      $��* �6'   .+�5�      �� 
�������$  5��6,   $ 50,20q�6=   $��0,0,25,0q�6M   ._�5�      �� $��Center=�6!   .+�5�      � .+�5�      �� $��
0,-2.5,0,0q�6)   .+�5�   "   �� -	   5        2 ��Name$2 EditéBorder��5�   Z   #��[ 6)   $��22��6o   $��22��6z   $  15��6�   #��[ 6�   $��2q�6�   ._�5�   &     -
   5        $��
EditéIcon��5�   U   $��10��6>   $��10��6I   $1 	CheckBold=�6f   $��Center=�6w   $��Center=�6�   
�������6�   5�   #   5�      �� 5�      $
��15,0q�6)   .+�5�   "   �� -   5        $��
EditéText��5�   �   $��0,0,0,5q�6,   $��Medium"�6=   $��13(�6Q   #��[ 6_   Edité6�   �� 5�   "   -   5        $��EditéDescription��5�   O   $��12(�6,   ���#a4b0be" Sélectionner la date d'édition6�   5�      5�      � 5�      $��	0,-15,0,0q�6    5�      �� 5�   "   $��12.218 �63   �� 5�   "   $��9.782 �63   �� 5�   "   $
��Auto �63    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   .+�5�      �� -   5        $��RejetéLine��5�   =   $	��2.5��6%   $��40��61   #��[ 6O   $��10,5,0,5q�6�   6�   �� 5�      -   5        $��RejetéBorder��5�   �   $��1��6%   $	��Top=�6�   $��0��6�   
�������$��22��6�   $��22��6  $  15��6  ���#a4b0be$��2q�6;  $��2��6O  ._�5�   "     -   5        $��RejetéIcon��5�   Q   $��10��6:   $��10��6E   $1 Circle=�6c   $��Center=�6q   $��Center=�6�   ���#a4b0be6�   5�      �� 5�      $��1��6)   $��2��66   $��	15,3,15,2q�6F   .+�5�   "   �� -   5        $��RejetéText��5�   =   $��0,0,0,5q�6,   $��Medium"�6O   $��13(�6c   ���#a4b0be	Rejeté6�   �� 5�   "   -   5        $��RejetéDescription��5�   :   $��12(�6,   ���#a4b0be)'Entrez le motif de rejeté avec la date6�   5�      5�      � 5�      $��	0,-15,0,0q�6    5�      �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   .+�5�      �� -   5        $��CorrigéLine��5�   =   $	��2.5��6%   $��40��61   #��[ 6P   $	��0,5q�6�   6�   �� 5�      -   5        $��CorrigéBorder��5�   J   $��1��6%   $	��Top=�62   $��0��6_   
�������$��22��6�   $��22��6�   $  15��6�   ���#a4b0be$��2q�6�   ._�5�   "     -   5        $��CorrigéIcon��5�   E   $��10��6:   $��10��6X   $1 Circle=�6d   $��Center=�6r   $��Center=�6�   ���#a4b0be6�   5�      �� 5�      $��1��6)   $��1��66   $��15,2.5q�6F   .+�5�   "   �� -   5        $��CorrigéText��5�   =   $��0,0,0,5q�6,   $��Medium"�6P   $��13(�6d   ���#a4b0be
Corrigé6�   �� 5�   "   -   5        $��CorrigéDescription��5�   :   $��12(�6,   ���#a4b0be#!Sélectionnez la date de corrigé6�   5�      5�      � 5�      $��	0,-15,0,0q�6    5�      �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   .+�5�      �� -   5        $��EnvoyéLine��5�   =   $	��2.5��6%   $��40��61   #��[ 6O   $	��0,5q�6�   6�   �� 5�      -   5        $��EnvoyéBorder��5�   �   $��1��6�   $	��Top=�6�   $��0��6�   ���#57606f$��22��6�   $��22��6	  $  15��6  ���#57606f$��2q�6=  ._�5�   "     -   5        $��EnvoyéIcon��5�   Q   $��10��6:   $��10��6E   $1 Circle=�6c   $��Center=�6q   $��Center=�6�   
�������6�   5�      �� 5�      $��1��6)   $��1��66   $��15,2.5q�6F   .+�5�   "   �� -   5        $��EnvoyéText��5�   =   $��0,0,0,5q�6,   $��Medium"�6O   $��13(�6c   ���#57606f	Envoyé6�   �� 5�   "   -   5        $��EnvoyéDescription��5�   :   $��12(�6,   ���#a4b0be!Sélectionnez la date d'envoyé6�   5�      5�      � 5�      $��	0,-15,0,0q�6    5�      �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   .+�5�      �� -   5        $��
SignéLine��5�   =   $	��2.5��6%   $��40��61   ���#a4b0be$	��0,5q�6c   6r   �� 5�      -   5        $��SignéBorder��5�   J   $��1��6%   $	��Top=�62   $��0��6]   
�������$��22��6�   $��22��6�   $  15��6�   ���#a4b0be$��2q�6�   ._�5�   "     -   5        $��
SignéIcon��5�   E   $��10��6:   $��10��6V   $1 Circle=�6b   $��Center=�6p   $��Center=�6�   ���#a4b0be6�   5�      �� 5�      $��1��6)   $��1��66   $��15,2.5q�6F   .+�5�   "   �� -   5        $��
SignéText��5�   =   $��0,0,0,5q�6,   $��Medium"�6N   $��13(�6b   ���#a4b0beSigné6�   �� 5�   "   -    5        $��SignéDescription��5�   :   $��12(�6,   ���#a4b0be!Sélectionnez la date de signé6�   5�      5�      5�      5�      �� 5�      $��1��6   
�������$  5��6<   $ 30q�6M   ._�5�      �  5�      �� 5�      $��* �6,   �� 5�      $
��Auto �6,   .+�5�      �� $��Center=�6%   $��Center=�6B   .+�5      �� 
����������#a4b0be$��1q�6N   $  5��6b   $ 20q�6s   ._�5  "   &   System.Windows.Controls.Calendar  -!   5        3  Name$3 Calendar��5  #   4 � Language$4 fr-FR�5  +   
�������5 J� Opacity$5 .4Q�5  '   -  'MaterialDesignThemes.Wpf.CalendarAssist6   Orientation$6 
Horizontal=�5  #    ,
 (MaterialDesignCalendarPortraitForeground#��[
 5	     5
     �� 5     -"   5        $��Motif��5  !   $ .9Q�5  !    ! MaterialDesignOutlinedTextBox#��[ 5     7 �� TextWrapping$
7 Wrap=�5     $��0,25,0,0q�5  !   $	��200��5     $��12,11q�5     $��12(�5     ���#57606f���#a4b0be �#a4b0be8   
Background
8 �����$
 Motif de rejeté��5     � 5     -#   5        $ ReEditer��5  5   $��0,25,0,0q�6#    5  "   �� 5  &   $��* �67   �� 5  &   $
��Auto �67    5  "     WarningIcon  WarningText  
WrapMargin��%�  &	       &	 �     &	 e    �� 5  &   "%  6,   .�5   *   �� ' � ��( �#d9254e6G   �� 5!  *   ' � ��$( 13��6B   �� 5"  *   ' � ��$( 13��6C   �� 5#  *   ' � ��$( 0,0,7,0q�6C   �� 5$  *   ' �@ Kind$( WarningCircle=�6A   5%  '   �� 5&  &   "% ��6,   .�5'  *   �� ' � ��$( 12(�6E   �� 5(  *   ' � ��( �#d9254e6G   5)  '   �� 5*  &   "% �6,   .�5+  *   �� ' � ��$( 0,5,0,0q�6C   5,  '   5-  #   .+�5.  "   �� $��0,0,25,0q�6-   .+�5/  &   (  "System.Windows.Controls.DatePicker  -$   5        $��ReEditerDate��5/  ^   $ .9Q�61    $  MaterialDesignOutlinedDatePicker#��[ 6r   .  (MaterialDesignThemes.Wpf.TextFieldAssist9   UnderlineBrush9 �#a4b0be
8 ����� �#a4b0be$	��200��6@  $��12,11q�6O  ���#a4b0be$��1,1,1,1q�6u  ���#5f6368$��12(�6�  $
 Date de re-éditer��6�   50  *   ��%)   &	       �� 51  .   $ White��6Z   52  +   53  '   � 54  &   -%   5        $��ReEditerDateWarning��54  G   $y�	Collapsed=�60   #��[ 6b   .+�55  *     #��[ 6B   �� 56  *   -&   5        $��ReEditerDateWarningText��56  4   #��[ 6S   57  '   58  #   �� 5:  "   -'   5        $��1��5:  )   $	��Top=�69   $��Right=�5;  !    ! MaterialDesignFlatMidBgButton#��[ 5=     $0 5��5>     $'+ !Ajouter un stagiaire manuellement��5?     $��12(�5@     $��36��5A     $��	20,0,15,1q�5B     {�5C     �� *DataContext.ControlsEnabled+: �� RelativeSource: �� *FindAncestor+; �� AncestorType$; WindowL���5D  &   � 5E  *   .+�5F  .   �� 
Re-éditer6C     5G  .   $1 	PencilAdd=�6F   $��10,0,0,0q�6W   $��14��6i   $��14��6t   5H  +   5J  #   5K     5L     �� 5O     -(   5        $��Valider��5R  !   $��1��5O  !   $��Right=�5P  !   #��[ 5S     $0 5��5T     $'+ !Ajouter un stagiaire manuellement��5U     $��12(�5V     $��36��5W     $��	20,2,15,3q�5X     {�5Y     �� *DataContext.ControlsEnabled+: �� *FindAncestor+$; WindowL���5Z     � 5[  "   .+�5\  &   �� 	Valider69     5]  &   $1 CalendarCheck=�6>   $��10,0,0,0q�6S   $��16��6e   $��16��6p   5^  #   5`     5a     5b     5c     5d     5e     5f     