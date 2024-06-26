import { GetSelectedStationIds } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetSelectedStationIds = createAsyncThunk('cache/getGetSelectedStationIds', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSelectedStationIds');
    const response = await GetSelectedStationIds();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


