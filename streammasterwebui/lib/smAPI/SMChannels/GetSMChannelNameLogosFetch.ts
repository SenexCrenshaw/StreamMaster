import { GetSMChannelNameLogos } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMChannelNameLogos = createAsyncThunk('cache/getGetSMChannelNameLogos', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSMChannelNameLogos');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetSMChannelNameLogos();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetSMChannelNameLogos completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


