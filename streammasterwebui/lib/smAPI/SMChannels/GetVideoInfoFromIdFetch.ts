import { GetVideoInfoFromId } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { GetVideoInfoFromIdRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetVideoInfoFromId = createAsyncThunk('cache/getGetVideoInfoFromId', async (param: GetVideoInfoFromIdRequest, thunkAPI) => {
  try {
    console.log('Fetching GetVideoInfoFromId');
    const response = await GetVideoInfoFromId(param);
    console.log('Fetched GetVideoInfoFromId',response);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


