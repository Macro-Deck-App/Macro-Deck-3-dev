using MacroDeck.SDK.UI.Components;
using MacroDeck.SDK.UI.Components.Input;
using MacroDeck.SDK.UI.Components.Layout;
using MacroDeck.SDK.UI.Core;
using MacroDeck.SDK.UI.Registry;
using Serilog;

namespace MacroDeck.Server.Integrations.Spotify.Views.Plugin;

[MdUiView(ViewId = "spotify.IntegrationConfigurationView")]
public class SpotifyIntegrationConfigurationView : StatefulMdUiView
{
	public override MdUiState CreateState()
	{
		return new SpotifyIntegrationConfigurationState();
	}
}

public class SpotifyIntegrationConfigurationState : MdUiState
{
	private State<string> _clientId = null!;
	private State<string> _clientSecret = null!;
	private State<int> _currentStep = null!;
	private State<string> _errorMessage = null!;

	public override void InitState()
	{
		_currentStep = CreateState(0);
		_clientId = CreateState("");
		_clientSecret = CreateState("");
		_errorMessage = CreateState("");
	}

	public override MdUiView Build()
	{
		return new MdContainer
		{
			Child = new MdColumn(BuildHeader(),
				BuildStepContent(),
				BuildNavigation())
			{
				Spacing = 0
			}
		};
	}

	private static MdUiView BuildHeader()
	{
		return new MdContainer
		{
			Child = new MdColumn(new MdText("Spotify Integration Setup")
			{
				FontSize = 32,
				FontWeight = FontWeight.Bold,
				Color = "#1DB954"
			})
			{
				Spacing = 0
			}
		};
	}

	private MdUiView BuildStepIndicator()
	{
		var steps = new[]
		{
			("1", "Introduction"),
			("2", "Create App"),
			("3", "Credentials"),
			("4", "Complete")
		};

		var stepViews = new List<MdUiView>();

		for (var i = 0; i < steps.Length; i++)
		{
			stepViews.Add(BuildStepDot(i, steps[i].Item1, steps[i].Item2));
			if (i < steps.Length - 1)
			{
				stepViews.Add(new MdContainer
				{
					Width = 50,
					Height = 3,
					BackgroundColor = _currentStep.Value > i ? "#1DB954" : "#E0E0E0",
					Margin = EdgeInsets.Only(left: 8, right: 8, top: 15)
				});
			}
		}

		return new MdRow(stepViews.ToArray())
		{
			MainAxisAlignment = MainAxisAlignment.Center,
			Margin = EdgeInsets.Only(20, bottom: 20)
		};
	}

	private MdUiView BuildStepDot(int step, string number, string label)
	{
		var isActive = _currentStep.Value == step;
		var isCompleted = _currentStep.Value > step;
		var bgColor = isActive ? "#1DB954" : isCompleted ? "#4CAF50" : "#E0E0E0";
		var textColor = isActive || isCompleted ? "#FFFFFF" : "#666666";

		return new MdColumn(new MdContainer
			{
				Width = 36,
				Height = 36,
				BorderRadius = BorderRadius.Circular(18),
				BackgroundColor = bgColor,
				Padding = EdgeInsets.All(8),
				Child = new MdText(isCompleted ? "✓" : number)
				{
					FontSize = 16,
					FontWeight = FontWeight.Bold,
					Color = textColor,
					TextAlign = TextAlign.Center
				}
			},
			new MdText(label)
			{
				FontSize = 11,
				Color = isActive ? "#1DB954" : "#666666",
				TextAlign = TextAlign.Center,
				Margin = EdgeInsets.Only(6)
			})
		{
			CrossAxisAlignment = CrossAxisAlignment.Center,
			Spacing = 0
		};
	}

	private MdUiView BuildStepContent()
	{
		return new MdContainer
		{
			Child = _currentStep.Value switch
			{
				0 => BuildIntroductionStep(),
				1 => BuildCreateAppStep(),
				2 => BuildCredentialsStep(),
				3 => BuildCompleteStep(),
				_ => BuildIntroductionStep()
			}
		};
	}

	private MdUiView BuildIntroductionStep()
	{
		return new MdColumn(
			new MdText("To integrate Spotify with Macro Deck, you'll need to create a Spotify Developer Application.")
			{
				FontSize = 14,
				Margin = EdgeInsets.Only(bottom: 20)
			},
			new MdText("Requirements:")
			{
				FontSize = 16,
				FontWeight = FontWeight.Bold,
				Margin = EdgeInsets.Only(bottom: 12)
			},
			new MdContainer
			{
				Child = new MdColumn(
					new MdText("• A Spotify account (Free or Premium, Free accounts have some limitations)")
					{
						FontSize = 14,
						Margin = EdgeInsets.Only(bottom: 8, left: 20)
					},
					new MdText("• Access to Spotify Developer Dashboard")
					{
						FontSize = 14,
						Margin = EdgeInsets.Only(bottom: 8, left: 20)
					},
					new MdText("• 5 minutes of your time")
					{
						FontSize = 14,
						Margin = EdgeInsets.Only(left: 20, bottom: 20)
					})
			},
			new MdContainer
			{
				Padding = EdgeInsets.All(16),
				BorderRadius = BorderRadius.Circular(8),
				Child = new MdColumn(new MdText("📝 Note:")
					{
						FontSize = 14,
						FontWeight = FontWeight.Bold,
						Margin = EdgeInsets.Only(bottom: 8)
					},
					new MdText("This setup is required only once. Your credentials will be stored securely.")
					{
						FontSize = 13,
						Color = "#2E7D32"
					})
				{
					Spacing = 0
				}
			})
		{
			Spacing = 0
		};
	}

	private MdUiView BuildCreateAppStep()
	{
		return new MdColumn(new MdText("Follow these steps to create your Spotify Developer Application:")
			{
				FontSize = 14,
				Margin = EdgeInsets.Only(bottom: 25)
			},
			BuildNumberedStep("1",
				"Open Spotify Developer Dashboard",
				"Visit the Spotify Developer Dashboard and log in with your Spotify account."),
			new MdButton("Open Spotify Developer Dashboard →",
				async () =>
				{
					Log.Logger.Information("Opening Spotify Developer Dashboard");
					var approved = await OpenLink("https://developer.spotify.com/dashboard");
					if (approved)
					{
						Log.Logger.Information("User approved opening Spotify Developer Dashboard");
					}
					else
					{
						Log.Logger.Information("User denied opening Spotify Developer Dashboard");
					}
				})
			{
				Role = ButtonRole.Primary,
				Margin = EdgeInsets.Only(bottom: 25, left: 45)
			},
			BuildNumberedStep("2",
				"Create a New Application",
				"Click on 'Create app' button in the dashboard."),
			BuildNumberedStep("3",
				"Fill in Application Details",
				"Enter: App Name: Macro Deck, App Description: Integration for Macro Deck, Redirect URI: http://127.0.0.1:8191/api/integrations/spotify/callback"),
			BuildNumberedStep("4",
				"Accept Terms and Create",
				"Check the terms and conditions, then click Save."))
		{
			Spacing = 0
		};
	}

	private MdUiView BuildCredentialsStep()
	{
		return new MdColumn(new MdText("Enter Your Credentials")
			{
				FontSize = 24,
				FontWeight = FontWeight.Bold,
				Margin = EdgeInsets.Only(bottom: 20)
			},
			new MdText("After creating your application, you'll find your credentials in the app settings.")
			{
				FontSize = 14,
				Margin = EdgeInsets.Only(bottom: 25)
			},
			BuildNumberedStep("1",
				"Open Application Settings",
				"Click on Settings in your newly created application."),
			BuildNumberedStep("2",
				"Find Your Credentials",
				"Copy the Client ID and click View client secret to reveal and copy the Client Secret."),
			new MdText("Enter your credentials below:")
			{
				FontSize = 16,
				FontWeight = FontWeight.Bold,
				Margin = EdgeInsets.Only(bottom: 16)
			},
			new MdTextField
			{
				Label = "Client ID",
				Placeholder = "Enter your Spotify Client ID...",
				Value = _clientId.Value,
				OnChanged = value => _clientId.Value = value,
				Margin = EdgeInsets.Only(bottom: 16),
				Padding = EdgeInsets.All(12)
			},
			new MdTextField
			{
				Label = "Client Secret",
				Placeholder = "Enter your Spotify Client Secret...",
				Value = _clientSecret.Value,
				OnChanged = value => _clientSecret.Value = value,
				Sensitive = true,
				Margin = EdgeInsets.Only(bottom: 16),
				Padding = EdgeInsets.All(12)
			},
			new MdContainer
			{
				Visible = _errorMessage.Value != "",
				Padding = EdgeInsets.All(12),
				BorderRadius = BorderRadius.Circular(8),
				BackgroundColor = "#FFEBEE",
				Child = new MdText(_errorMessage.Value)
				{
					FontSize = 14,
					Color = "#C62828"
				}
			})
		{
			Spacing = 0
		};
	}

	private MdUiView BuildCompleteStep()
	{
		return new MdColumn(new MdContainer
			{
				Height = 100,
				Width = 100,
				Margin = EdgeInsets.Only(bottom: 20, top: 20),
				BorderRadius = BorderRadius.Circular(50),
				Padding = EdgeInsets.All(20),
				Child = new MdText("✓")
				{
					FontSize = 50,
					Color = "#1DB954",
					TextAlign = TextAlign.Center
				}
			},
			new MdText("Setup Complete!")
			{
				FontSize = 26,
				FontWeight = FontWeight.Bold,
				Margin = EdgeInsets.Only(bottom: 12),
				TextAlign = TextAlign.Center
			},
			new MdText("Your Spotify integration has been configured successfully.")
			{
				FontSize = 14,
				Color = "#666666",
				Margin = EdgeInsets.Only(bottom: 25),
				TextAlign = TextAlign.Center
			})
		{
			CrossAxisAlignment = CrossAxisAlignment.Center,
			Spacing = 0
		};
	}

	private MdUiView BuildNumberedStep(string number, string title, string description)
	{
		return new MdRow(new MdContainer
			{
				Width = 32,
				Height = 32,
				BorderRadius = BorderRadius.Circular(16),
				BackgroundColor = "#1DB954",
				Margin = EdgeInsets.Only(right: 12, top: 2),
				Padding = EdgeInsets.All(2),
				Alignment = Alignment.Center,
				Child = new MdText(number)
				{
					FontSize = 14,
					FontWeight = FontWeight.Bold,
					Color = "#FFFFFF"
				}
			},
			new MdColumn(new MdText(title)
				{
					FontSize = 15,
					FontWeight = FontWeight.Bold,
					Margin = EdgeInsets.Only(bottom: 6)
				},
				new MdText(description)
				{
					FontSize = 13,
					Color = "#666666"
				})
			{
				Spacing = 0
			})
		{
			CrossAxisAlignment = CrossAxisAlignment.Start,
			Margin = EdgeInsets.Only(bottom: 20)
		};
	}

	private MdUiView BuildNavigation()
	{
		var buttons = new List<MdUiView>();

		if (_currentStep.Value > 0)
		{
			buttons.Add(new MdButton("← Previous",
				() =>
				{
					_currentStep.Value--;
					_errorMessage.Value = "";
				})
			{
				Role = ButtonRole.Secondary,
				Margin = EdgeInsets.Only(right: 12)
			});
		}

		if (_currentStep.Value < 3)
		{
			buttons.Add(new MdButton(_currentStep.Value == 2 ? "Validate & Continue →" : "Next →",
				() =>
				{
					if (_currentStep.Value == 2)
					{
						ValidateCredentials();
					}
					else
					{
						_currentStep.Value++;
					}
				})
			{
				Role = ButtonRole.Primary
			});
		}
		else
		{
			buttons.Add(new MdButton("Finish Setup",
				() =>
				{
// TODO: Save configuration and close wizard
					Log.Logger.Information("Spotify integration setup completed");
				})
			{
				Role = ButtonRole.Success
			});
		}

		return new MdRow(buttons.ToArray())
		{
			MainAxisAlignment = MainAxisAlignment.End,
			Margin = EdgeInsets.Only(25)
		};
	}

	private void ValidateCredentials()
	{
		_errorMessage.Value = "";

		if (string.IsNullOrWhiteSpace(_clientId.Value))
		{
			_errorMessage.Value = "Please enter a Client ID";
			return;
		}

		if (string.IsNullOrWhiteSpace(_clientSecret.Value))
		{
			_errorMessage.Value = "Please enter a Client Secret";
			return;
		}

// TODO: Validate credentials with Spotify API
		Log.Logger.Information("Validating Spotify credentials: ClientId={ClientId}", _clientId.Value);

		_currentStep.Value++;
	}
}
