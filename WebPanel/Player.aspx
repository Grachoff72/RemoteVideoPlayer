<%@ Page CodeBehind="Player.aspx.cs" Inherits="WebPanel.Player" Async="true" Language="C#" %>
<%--<%@ Register TagPrefix="controls" namespace="WebPanel" assembly="RemoteVideoPlayerWebPanel" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta name="viewport" content="width=device-width, height=device-height, initial-scale=1.0"/>
	<title>Player</title>
	<style type="text/css">
		.auto-style2 {
			width: 60px;
			height: 60px;
			text-align: center
		}
		.image-button {
			width: 50px;
			text-align: center
		}
		  table.center {
			width:80%; 
			margin-left:10%; 
			margin-right:10%;
		  }
		/*.marquee {
		  overflow: hidden;
		  height: 2em;
		}
		@keyframes marquee {
		  from {
			text-indent: 0;
		  }
		  to {
			text-indent: -100%;
		  }
		}
		.auto-style4 {
			overflow: hidden;
			animation: 30s linear 1s infinite alternate backwards marquee;
		}*/
	</style>
</head>
<body style="width: 240px; height: 380px; margin-bottom: 0; background: #000000; text-align: center">
<form id="form1" runat="server">
	<asp:ScriptManager ID="ButtonManager" runat="server" EnablePartialRendering="True"/>
	<asp:UpdatePanel ID="ButtonPanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
		<ContentTemplate>
			<table>
				<tr>
					<td class="auto-style2">
						<asp:ImageButton ID="InfoButton" runat="server" CssClass="image-button" ImageUrl="Content/info.png" OnClick="InfoButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="PowerButton" runat="server" CssClass="image-button" ImageUrl="Content/power-button.png" OnClick="PowerButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="LevelUpButton" runat="server" CssClass="image-button" ImageUrl="Content/back.png" OnClick="LevelUpButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="CloseButton" runat="server" CssClass="image-button" ImageUrl="Content/close.png" OnClick="CloseButton_Click" />
					</td>
				</tr>
				<tr>
					<td class="auto-style2">
						<asp:ImageButton ID="VolumeUpButton" runat="server" CssClass="image-button" ImageUrl="Content/volumeup.png" OnClick="VolumeUpButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="ScreenButton" runat="server" CssClass="image-button" ImageUrl="Content/expand.png" OnClick="ScreenButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="VolumeButton" runat="server" CssClass="image-button" ImageUrl="Content/volume.png" OnClick="VolumeButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="PageUpButton" runat="server" CssClass="image-button" ImageUrl="Content/pageup.png" OnClick="PageUpButton_Click" />
					</td>
				</tr>
				<tr>
					<td class="auto-style2">
						<asp:ImageButton ID="VolumeDownButton" runat="server" CssClass="image-button" ImageUrl="Content/volumedown.png" OnClick="VolumeDownButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="PlayButton" runat="server" CssClass="image-button" ImageUrl="Content/play.png" OnClick="PlayButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="PauseButton" runat="server" CssClass="image-button" ImageUrl="Content/pause.png" OnClick="PauseButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="PageDownButton" runat="server" CssClass="image-button" ImageUrl="Content/pagedown.png" OnClick="PageDownButton_Click" />
					</td>
				</tr>
			</table>
			<table class="center">
				<tr>
					<td class="auto-style2">
						<asp:ImageButton ID="PrevButton" runat="server" CssClass="image-button" ImageUrl="Content/previous.png" OnClick="PrevButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="UpButton" runat="server" CssClass="image-button" ImageUrl="Content/up-arrow.png" OnClick="UpButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="NextButton" runat="server" CssClass="image-button" ImageUrl="Content/next.png" OnClick="NextButton_Click" />
					</td>
				</tr>
				<tr>
					<td class="auto-style2">
						<asp:ImageButton ID="LeftButton" runat="server" CssClass="image-button" ImageUrl="Content/left-arrow.png" OnClick="LeftButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="OkButton" runat="server" CssClass="image-button" ImageUrl="Content/ok.png" OnClick="OkButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="RightButton" runat="server" CssClass="image-button" ImageUrl="Content/right-arrow.png" OnClick="RightButton_Click" />
					</td>
				</tr>
				<tr>
					<td class="auto-style2">
						<asp:ImageButton ID="RewindButton" runat="server" CssClass="image-button" ImageUrl="Content/rewind.png" OnClick="RewindButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="DownButton" runat="server" CssClass="image-button" ImageUrl="Content/down-arrow.png" OnClick="DownButton_Click" />
					</td>
					<td class="auto-style2">
						<asp:ImageButton ID="ForwardButton" runat="server" CssClass="image-button" ImageUrl="Content/fast-forward.png" OnClick="ForwardButton_Click" />
					</td>
				</tr>
			</table>
		</ContentTemplate>
	</asp:UpdatePanel>
</form>
</body>
</html>