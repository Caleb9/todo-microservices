# docker-compose convenience scripts. docker-compose command needs
# to be available in the system
up:
	@docker-compose up --build

down:
	@docker-compose down --rmi local

clean:
	@docker-compose down --rmi local -v
