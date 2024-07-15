import { GetSMTasks } from '@lib/smAPI/SMTasks/SMTasksCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMTasks = createAsyncThunk('cache/getGetSMTasks', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSMTasks');
    const response = await GetSMTasks();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});

