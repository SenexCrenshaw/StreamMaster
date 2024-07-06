import { GetClientStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


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


