import { GetStreamGroupSMChannels } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import { GetStreamGroupSMChannelsRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetStreamGroupSMChannels = createAsyncThunk('cache/getGetStreamGroupSMChannels', async (param: GetStreamGroupSMChannelsRequest, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStreamGroupSMChannels');
    const response = await GetStreamGroupSMChannels(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


