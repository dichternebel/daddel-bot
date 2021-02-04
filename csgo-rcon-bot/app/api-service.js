const superagent = require('superagent');

module.exports =
{
    get: async(endpoint, payload) => {
        return await superagent.get(endpoint.url).send({param: payload.param}).set('x-functions-key', endpoint.authKey).set('accessToken', payload.accessToken).set('salt', payload.salt);
    },
    getJson: async(endpoint, payload) => {
        return await superagent.get(endpoint.url).type('json').send(payload.param).set('x-functions-key', endpoint.authKey).set('accessToken', payload.accessToken).set('salt', payload.salt);
    },
    postJson: async(endpoint, jsonObj) => {
        return await superagent.post(endpoint.url).type('json').send(jsonObj).set('x-functions-key', endpoint.authKey).set('accessToken', jsonObj.accessToken).set('salt', jsonObj.salt);
    },
    delete: async(endpoint, payload) => {
        return await superagent.delete(endpoint.url).send(payload.param).set('x-functions-key', endpoint.authKey).set('accessToken', payload.accessToken).set('salt', payload.salt);
    }
};