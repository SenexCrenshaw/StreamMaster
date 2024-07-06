import { GetSMChannelStreams } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import { GetSMChannelStreamsRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMChannelStreams = createAsyncThunk('cache/getGetSMChannelStreams', async (param: GetSMChannelStreamsRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetSMChannelStreams');
    const response = await GetSMChannelStreams(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


