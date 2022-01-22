package main

type createBaseRequest struct {
	companyShortName string
	resourceGroup    string
	containers       []string
}

func newBaseRequest() (request *createBaseRequest) {
	return &createBaseRequest{
		companyShortName: "ks",
		resourceGroup:     "base",
		containers: []string{
			"mgmt",
			"dev",
			"uat",
			"prod",
			"iam",
			"adds",
			"er",
		},
	}
}
