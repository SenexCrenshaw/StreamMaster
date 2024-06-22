import { GetLineups } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetLineups = createAsyncThunk('cache/getGetLineups', async (_: void, thunkAPI) => {
  try {
    const response = await GetLineups();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


