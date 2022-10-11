import socket, sys, time

def main():
	time.sleep(5)
	#HOST = sys.argv[1]
	HOST = "127.0.0.1"
	PORT = 11111
	with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
		s.connect((HOST, PORT))
		while True:
			s.sendall(b"Hello, world<E>")
			time.sleep(0.2)

if __name__ == "__main__":
	main()