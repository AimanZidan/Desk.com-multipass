Desk.com-multipass
==================

Desk.com multipass example in C#

What is Multipass?
Multipass authentication is a single sign on authentication strategy to allow you to share your user authentication with your Desk.com site. This allows a seamless experience for your users without forcing them to create a separate account on your Desk.com site.

How does it work?
In order to authenticate your users on your Desk.com portal, you pass an encrypted multipass JSON hash with the user's information to your Desk.com site. When the multipass token is received, an associated customer record on your Desk.com site is created or updated and logged in with the information you provided.


http://dev.desk.com/guides/sso/
