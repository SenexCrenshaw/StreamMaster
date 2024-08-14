import { GetSMChannelNameLogos } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMChannelNameLogos = createAsyncThunk('cache/getGetSMChannelNameLogos', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSMChannelNameLogos');
    const response = await GetSMChannelNameLogos();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


