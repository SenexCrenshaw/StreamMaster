import { GetChannelDistributors } from '@lib/smAPI/Statistics/StatisticsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelDistributors = createAsyncThunk('cache/getGetChannelDistributors', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelDistributors');
    const response = await GetChannelDistributors();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


