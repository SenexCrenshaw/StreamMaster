import { GetStationPreviews } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetStationPreviews = createAsyncThunk('cache/getGetStationPreviews', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStationPreviews');
    const response = await GetStationPreviews();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});

