A sample application which makes use of the Reventuous library to showcase event sourcing using Redis as the event store.

The sample exposes 2 endpoints:
- create a bank account
- credit the account

When a bank account is created, an email is sent to the account owner (using SendGrid).
This is done by using reactions, where the application reacts to AccountCreated events.

The application follows the Onion architecture:
- Domain (where the domain logic is, e.g. the bank account aggregate)
- Application (where the controller and services reside)
- Infrastructure (where services that talk to external systems, e.g. SendGrid, live)

The react to events, a permanent subscription is created which subscribes to the "$bycategory-account" stream.
To get this stream created, the following Python function needs to be registered as a Redis Gears function:

```
def project(x):
    if x['key'].startswith('$') is False:
        # $all projection
        execute('xadd', '$all', '*', 'stream', x['key'], 'position', x['id'])

        # $by-category:<category>, e.g. for streams like "Account:<guid>" a "$by-category:Account" stream will be created
        category = x['key'].split(':')
        execute('xadd', '$by-category:' + category[0], '*', 'stream', x['key'], 'position', x['id'])

        # $by-event-type:<event-type>, e.g. for an event AccountCreated a "$by-event-type:AccountCreated" stream will be created
        execute('xadd', '$by-event-type:' + x['value']['type'], '*', 'stream', x['key'], 'position', x['id'])

gb = GearsBuilder('StreamReader')
gb.foreach(project)
gb.register(prefix='*', duration=1, batch=1, trimStream=False)

```

Save this to a file, e.g. projections.py, and load this into redis: 
$ redis-cli rg.pyexecute "`cat projections.py`"


User swagger UI to send commands to the 2 end points: http://localhost:5000/swagger