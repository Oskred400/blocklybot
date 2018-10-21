goog.require('Blockly.Blocks');
goog.require('Blockly');

Blockly.defineBlocksWithJsonArray(
[
    {
        "type": "send_sms",
        "message0": "Send Message %1",
        "args0": [
            {
                "type": "input_value",
                "name": "message_text",
                "check": "String"
            }
        ],
        "previousStatement": null,
        "nextStatement": null,
        "colour": 230,
        "tooltip": "Send a message to the user",
        "helpUrl": ""
    },
    {
        "type": "receive_sms",
        "lastDummyAlign0": "CENTRE",
        "message0": "Receive Message",
        "nextStatement": null,
        "colour": 230,
        "tooltip": "Executes when a message is received from the user",
        "helpUrl": ""
    },
    {
        "type": "get_sms_body",
        "lastDummyAlign0": "RIGHT",
        "message0": "Message Text",
        "output": "String",
        "colour": 230,
        "tooltip": "Gets the message of the text that was received",
        "helpUrl": ""
    },
    {
        "type": "get_sms_sender",
        "message0": "Message Sender",
        "output": "String",
        "colour": 230,
        "tooltip": "Get the sending phone number of the message that was received",
        "helpUrl": ""
    }
]);
