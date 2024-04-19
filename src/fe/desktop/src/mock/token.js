import * as jose from 'jose'

import { log } from '@/utils/index.js'

import Mock from '../lib/better-mock/mock.browser.esm.js'

const secret = new TextEncoder().encode(
  'cc7e0d44fd473002f1c42167459001140ec6389b7353f8088f4d9a95f2f596f2',
)
const alg = 'HS256'

const issuer = 'urn:example:issuer' // 发行方
const audience = 'urn:example:audience' // 接收方
const accessTokenTimeout = '1m'
const refreshTokenTimeout = '10m'

async function createToken(claims, timeout) {
  const jwt = await new jose.SignJWT(claims)
    .setProtectedHeader({ alg })
    .setIssuedAt()
    .setIssuer(issuer)
    .setAudience(audience)
    .setExpirationTime(timeout)
    .sign(secret)
  return jwt
}

export default function () {
  Mock.mock('/api/token/create', 'POST', request => {
    log(`mock:${request.url}`)
    const { userName, password } = JSON.parse(request.body ?? '{}')

    if (!userName) {
      return { code: 400, errors: { userName: ['用户名不能为空'] } }
    }
    if (!password) {
      return { code: 400, message: '密码不能为空' }
    }
    if (userName === 'admin' && password === '123456') {
      const claims = { user: userName }
      return new Promise(resolve => {
        Promise.all([
          createToken(claims, accessTokenTimeout),
          createToken(claims, refreshTokenTimeout),
        ]).then(results => {
          const [access_token, refresh_token] = results
          resolve({
            access_token,
            refresh_token,
          })
        })
      })
    }
    return {
      code: 400,
      message: '用户名或密码错误',
      data: { '': '用户名或密码错误' },
    }
  })

  Mock.mock('/api/token/refresh', 'POST', request => {
    log(`mock:${request.url}`)
    const jwt = JSON.parse(request.body)
    return new Promise(resolve => {
      jose
        .jwtVerify(jwt, secret, { issuer, audience })
        .then(_ => {
          const claims = { user: 'admin' }
          Promise.all([
            createToken(claims, accessTokenTimeout),
            createToken(claims, refreshTokenTimeout),
          ]).then(results => {
            const [access_token, refresh_token] = results
            resolve({
              access_token,
              refresh_token,
            })
          })
        })
        .catch(_ => {
          resolve({ _status: 400, code: 400, message: 'refresh_token 已过期' })
        })
    })
  })
}
