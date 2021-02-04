module.exports =
{
	name: "moin",
	aliasses: ["rush", "rash", "cyka", "blyat", "сука", "блядь", "hi", "hello", "howdy", "hallo", "huhu", "tach", "na", "salut", "приве́т", "алло", "whoami"],
	execute: (config, discordConfig, service, command, args) =>
	{
		if (command === "moin") {
			service.sendMessageToChannel("Ja, moin du alte Hackfresse!\nAlles fit im Schritt? :joy:");
			return;
		}
		if (command === "rush" || command === "rash" || command === "cyka"
			|| command === "blyat" || command === "сука" || command === "блядь") {
			service.sendMessageToChannel(":rofl: **Сука блядь " + service.message.author.username + "!!!**\n\nhttps://www.youtube.com/watch?v=8I8N4Me5r1I");
			return;
		}
        if (command === "hi" || command === "hello" || command === "howdy") {
			service.sendMessageToChannel("Hi there! :wave:");
			return;
		}
		if (command === "hallo" || command === "huhu" || command === "tach" || command ==="na") {
			service.sendMessageToChannel("Hallo, na!? :wave:");
			return;
		}
		if (command ==="salut") {
			service.sendMessageToChannel("Salut! Ça va? :wave:");
			return;
		}
		if (command === "приве́т" || command == "алло") {
			service.sendMessageToChannel("Алло! :wave:");
			return;
		}
		if (command ==="whoami") {
			service.sendMessageToChannel("Uhmm... :exploding_head: ... well at the moment u r *" + service.message.author.username + "* !?\nIs this of any help? :zany_face:");
			return;
		}
	}
};