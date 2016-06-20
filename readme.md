# Simple ticketing system in MVC (AJAX)

Single Sign-On
Detect user private ip address
Detect hostname
## Detect user location(Locations should be defined according subnet mask at file Models\computer.cs) 
"private string GetLocation() 
{ 
  string loc; switch (_octet) 
  { 
    case 1: loc = "Sede"; 
      break; 
    case 3: loc = "Poente"; 
      break; 
    case 4: loc = "Piscina"; 
      break; 
    case 5: loc = "Nascente"; 
      break; 
    default: loc = ""; break; 
  } 
  return loc; 
}"

##2 options to report a ticket
###- IN (incident) - report a problem
###- CH (change) - report a change or installation
#2 types of users, Admin and non-admin Admin can see all reported tickets and send message to the user non-admin can see their opened tickets

##Everytime someone create a ticket, Admin's receive an e-mail These Admin e-mails can be configured

##Outputs are written in portuguese and can be changed
