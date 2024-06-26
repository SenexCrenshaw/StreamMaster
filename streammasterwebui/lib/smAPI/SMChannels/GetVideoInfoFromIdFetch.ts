import { GetVideoInfoFromId } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { GetVideoInfoFromIdRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetVideoInfoFromId = createAsyncThunk('cache/getGetVideoInfoFromId', async (param: GetVideoInfoFromIdRequest, thunkAPI) => {
  try {
    Logger.debug('Fetching GetVideoInfoFromId');
    const response = await GetVideoInfoFromId(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


