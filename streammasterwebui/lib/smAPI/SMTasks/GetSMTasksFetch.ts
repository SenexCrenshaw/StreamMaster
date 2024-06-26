import { GetSMTasks } from '@lib/smAPI/SMTasks/SMTasksCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


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


