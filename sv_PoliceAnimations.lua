RegisterCommand('shield', function(source, args, rawCommand)
	TriggerClientEvent("PoliceAnimations:Shield", source)
end, false)

RegisterCommand('drunk', function(source, args, rawCommand)
	TriggerClientEvent("PoliceAnimations:Drunk", source)
end, false)
