import { GetChannelStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetChannelStreamingStatistics = createAsyncThunk('cache/getGetChannelStreamingStatistics', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelStreamingStatistics');
    const response = await GetChannelStreamingStatistics();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


