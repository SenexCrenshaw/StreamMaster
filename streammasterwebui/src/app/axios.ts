import axios from 'axios';
import { baseHostURL } from '../settings';

export default axios.create({
  baseURL: baseHostURL,
  headers: {
    'Content-type': 'application/json',
  },
});
