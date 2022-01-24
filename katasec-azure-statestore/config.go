package main

var (
	CompanyShortName = "kts"
)

type Config struct {
	SpokeNames []string

	ResourceGroupList []string
}

func NewHubConfig() *Config {
	config := &Config{
		SpokeNames: []string{
			"net",
			"vm",
			"aks",
			"kv",
		},
	}

	return config
}

func NewSpokeConfig() *Config {
	config := &Config{
		SpokeNames: []string{
			"net",
			"er",
			"wks",
			"acr",
			"kv",
		},
	}

	return config
}
