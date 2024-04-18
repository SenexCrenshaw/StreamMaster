import { GetStreamGroupSMChannels } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import { GetStreamGroupSMChannelsRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamGroupSMChannels = createAsyncThunk('cache/getGetStreamGroupSMChannels', async (param: GetStreamGroupSMChannelsRequest, thunkAPI) => {
  try {
    console.log('Fetching GetStreamGroupSMChannels');
    const response = await GetStreamGroupSMChannels(param);
    console.log('Fetched GetStreamGroupSMChannels ',response?.length);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


