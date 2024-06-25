import { GetSubscribedLineup } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetSubscribedLineup = createAsyncThunk('cache/getGetSubscribedLineup', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSubscribedLineup');
    const response = await GetSubscribedLineup();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


