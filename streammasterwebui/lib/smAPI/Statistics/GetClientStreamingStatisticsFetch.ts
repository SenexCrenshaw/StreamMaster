import { GetClientStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetClientStreamingStatistics = createAsyncThunk('cache/getGetClientStreamingStatistics', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetClientStreamingStatistics');
    const response = await GetClientStreamingStatistics();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


