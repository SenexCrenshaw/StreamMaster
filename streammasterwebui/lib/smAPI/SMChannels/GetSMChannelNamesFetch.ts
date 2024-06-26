import { GetSMChannelNames } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetSMChannelNames = createAsyncThunk('cache/getGetSMChannelNames', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSMChannelNames');
    const response = await GetSMChannelNames();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


