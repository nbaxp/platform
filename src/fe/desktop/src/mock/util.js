import { jwtDecode } from 'jwt-decode'

import { log } from '@/utils/index.js'

const valid = (request, validToken = true) => {
  log(`mock:${request.url}`)
  if (validToken) {
    const token = request.headers.authorization.split(' ')[1]
    const { exp } = jwtDecode(token)
    if (new Date(exp * 1000) < new Date()) {
      return { _status: 401 }
    }
  }
  return null
}

export default valid
