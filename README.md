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
To get this stream created, the following Python function needs to be registered as a Redis Gears function, using redisinsight web interface (http://localhost:8001)

```
def project(x):
    # events are added to streams by RedisEventStore class, in a similar way to running these Redis commands
    # 
    #   xadd account:1234 * type AccountCreated data '{"AccountId":"3c8e510c-e00a-4b9b-abf4-b2a37152d625"}'
    #   xadd account:1234 * type AccountCredited data '{"Amount":50}'
    #
    # project only events for non-system streams (e.g. not starting with $)
    if x['key'].startswith('$') is False:
        # $all projection
        execute('xadd', '$all', '*', 'stream', x['key'], 'position', x['id'])

        # $by-category:<category>, e.g. for streams like "account:xxxx" we'll get a "$by-category:account" stream
        category = x['key'].split(':')
        execute('xadd', '$by-category:' + category[0], '*', 'stream', x['key'], 'position', x['id'])

        # $by-event-type:<event-type>, e.g. for an event AccountCreated we'll get a "$by-event-type:AccountCreated" stream
        execute('xadd', '$by-event-type:' + x['value']['type'], '*', 'stream', x['key'], 'position', x['id'])

gb = GearsBuilder('StreamReader')
gb.foreach(project)
gb.register(prefix='*', duration=1, batch=1, trimStream=False)

```
