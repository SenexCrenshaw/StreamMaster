import { GetVideoInfos } from '@lib/smAPI/Statistics/StatisticsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetVideoInfos = createAsyncThunk('cache/getGetVideoInfos', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetVideoInfos');
    const response = await GetVideoInfos();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


