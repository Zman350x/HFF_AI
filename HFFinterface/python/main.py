import socket, sys, time, pynput, json
from game_inputs import GameInputs

def main():
	#time.sleep(5)
	#HOST = sys.argv[1]
	HOST = "127.0.0.1"
	PORT = 11111

	hff_inputs = GameInputs()
	msg = hff_inputs.format_json()
	print(msg)

	with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
		s.connect((HOST, PORT))
		while True:
			s.sendall(f"{msg}<E>".encode())
			temp = s.recv(1024)
			#if temp:
				#msg = temp.decode("utf-8");
			time.sleep(0.2)

if __name__ == "__main__":
	main()