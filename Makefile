COMPOSE ?= docker-compose

compose-up:
	$(COMPOSE) up

compose-restart-api:
	$(COMPOSE) restart api

compose-migrate:
	DOTNET_ARGS="--migrate-db" $(COMPOSE) up api

compose-bootstrap:
	DOTNET_ARGS="--bootstrap-db" $(COMPOSE) up api

compose-setup-db:
	DOTNET_ARGS="--migrate-db --bootstrap-db" $(COMPOSE) up api

.PHONY: compose-up compose-migrate compose-bootstrap compose-setup-db compose-restart-api
