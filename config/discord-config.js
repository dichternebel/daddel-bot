module.exports =
{
    schema: {
        accessToken: {
            type: 'string',
        },
        server: {
            type: 'string',
        },
        port: {
            type: 'number',
            default: 27015
        },
        password: {
            type: 'string'
        },
        role: {
            type: 'string'
        },
        isEnabled: {
            type: 'boolean',
            default: false
        }
    }
};