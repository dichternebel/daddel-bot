module.exports =
{
    schema: {
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
        accessToken: {
            type: 'string'
        },
        salt: {
            type: 'string'
        },
        isEnabled: {
            type: 'boolean',
            default: false
        }
    }
};