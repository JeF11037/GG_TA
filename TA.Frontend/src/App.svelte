<script>
	import { onMount } from 'svelte';

	let zips;
	let zip_count;

	let error_message;
	let error_codes = [400, 500];

	onMount(async () => {
		try
		{
			const response = await fetch(
				'https://localhost:9090/api/zip/get',
				{
					method: 'GET'
				}
			);

			const data = await response.json();
			
			if (error_codes.includes(response.status))
				error_message = data;

			zips = data.zips;
			zip_count = zips.length;
		}
		catch (error)
		{
			error_message = error.toString();
		}
	});

	async function deleteZipOnClick(event)
	{
		try 
		{
			const response = await fetch(
				'https://localhost:9090/api/zip/delete'+'?zip_name='+event.target.getAttribute("data-zip_name"),
				{
					method: 'POST'
				}
			);

			const button = document.querySelector("[data-zip_name='"+event.target.getAttribute("data-zip_name")+"']");

			if (error_codes.includes(response.status))
				error_message = await response.json();
			else
				button.style = "display: none;"
		} 
		catch (error) 
		{
			error_message = error.toString();
		}
	}

	async function loadZipOnChange(event)
	{
		try 
		{
			const icon_sync = document.querySelector("[data-icon-sync]");
			const icon_close = document.querySelector("[data-icon-close]");
			const icon_check = document.querySelector("[data-icon-check]");

			icon_sync.style = "opacity: 1;";
			icon_close.style = "opacity: 0";
			icon_check.style = "opacity: 0";

			const tree_view = document.querySelector(".tree_view");

			const zip_picker = document.querySelector("[data-zip-picker]");
			let zip = new FormData();
			zip.append('file', zip_picker.files[0]);
			const response = await fetch(
				'https://localhost:9090/api/zip/validate',
				{
					method: 'POST',
					body: zip
				}
			);
			
			error_message = null;

			const data = await response.json();
			
			const tree_view_h2 = document.querySelector("[data-tree-view-h2]");

			const button_zip = document.querySelector("[data-button-zip]");

			if (error_codes.includes(response.status))
			{
				icon_sync.style = "opacity: 0";
				icon_close.style = "opacity: 1";
				icon_check.style = "opacity: 0";
				error_message = data.errors;
			}
			else
			{
				icon_sync.style = "opacity: 0";
				icon_close.style = "opacity: 0";
				icon_check.style = "opacity: 1";
				button_zip.disabled = false;
			}

			let tree = ``;

			const class_container = document.querySelector("[data-class-container]");
				
			const ul = (reverse = false) =>
			{
				return reverse ? `</ul>` : `<ul class=`+class_container.classList[0]+`>`;
			}

			const li = (reverse = false) =>
			{
				return reverse ? `</li>` : `<li class=`+class_container.classList[0]+`>`;
			}
 
			const file = (name) =>
			{
				return `<li class=`+class_container.classList[0]+`><span class=`+class_container.classList[0]+`><i class="material-symbols-outlined">draft</i> `+name+`</span></li>`;
			}

			const folder = (name) =>
			{
				return `<span class=`+class_container.classList[0]+`><i class="material-symbols-outlined">folder</i> `+name+`</span>`;
			}

			if (data.dlls.Files.length > 0)
			{
				tree += li();
				tree += folder("dlls");
				tree += ul();

				data.dlls.Files.forEach(file_name => {
					tree += file(file_name);
				});

				tree += ul(true);
				tree += li(true);
			}

			if (data.images.Files.length > 0)
			{
				tree += li();
				tree += folder("images");
				tree += ul();

				data.images.Files.forEach(file_name => {
					tree += file(file_name);
				});

				tree += ul(true);
				tree += li(true);
			}

			if (data.languages.Files.length > 0)
			{
				tree += li();
				tree += folder("languages");
				tree += ul();

				data.languages.Files.forEach(file_name => {
					tree += file(file_name);
				});

				tree += ul(true);
				tree += li(true);
			}

			if (data.dlls.Files.length > 0 && data.images.Files.length > 0 && data.languages.Files.length > 0)
				tree_view_h2.innerHTML = "Root is empty";
			else
				tree_view_h2.innerHTML = "Root";

			tree_view.innerHTML = tree;
		} 
		catch (error) 
		{
			error_message = error.toString();
		}
	}

	async function uploadZipOnClick(event)
	{
		try 
		{
			const zip_picker = document.querySelector("[data-zip-picker]");
			let zip = new FormData();
			zip.append('file', zip_picker.files[0]);
			const response = await fetch(
				'https://localhost:9090/api/zip/upload',
				{
					method: 'POST',
					body: zip
				}
			);

			if (error_codes.includes(response.status))
				error_message = await response.json();
			else
				location.reload();
		} 
		catch (error) 
		{
			error_message = error;
		}
	}

</script>

<main>
	<input type="hidden" data-class-container>
	<section>
		<div class="container">
			<div
				css-flex="column"
			>
				<input 
					type="file" 
					data-zip-picker
					class="h1"
					accept=".zip"
					on:change={loadZipOnChange}
				>
			</div>
			<div
				css-flex="space-between"
			>
				<div class="tree_view_container">
					<h2 data-tree-view-h2>Root</h2>
					<ul class="tree_view">
						<li>
							<span><i class="material-symbols-outlined">folder</i> Folder 1</span>
							<ul>
								<li><span><i class="material-symbols-outlined">draft</i> Subfile 1</span></li>
							</ul>
						</li>
						<li><span><i class="material-symbols-outlined">folder</i> Folder 2</span></li>
						<li><span><i class="material-symbols-outlined">folder</i> Folder 3</span></li>
						<li><span><i class="material-symbols-outlined">draft</i> File 1.jpg</span></li>
						<li><span><i class="material-symbols-outlined">draft</i> File 2.jpg</span></li>
					</ul>  
				</div>
				<div
					class="scan_icon"
					css-flex="margin-auto"
				>
					<i
						class="material-symbols-outlined rotate_animation"
						data-icon-sync
						css-font="10-rem"
					>
						sync
					</i>
					<i
						class="material-symbols-outlined"
						data-icon-close
						css-font="10-rem"
					>
						close
					</i>
					<i
						class="material-symbols-outlined"
						data-icon-check
						css-font="10-rem"
					>
						check
					</i>
				</div>
				<div
					css-flex="margin-auto"
				>
					<button data-button-zip disabled on:click={uploadZipOnClick}>Add to server</button>
				</div>
			</div>
		</div>
		<div class="container">
			{#if zip_count > 0}
				<div
					css-flex="column"
				>
					{#if zips}
						<h1>Click to delete a zip</h1>
						<div
							css-flex="wrap gap-1-2 jc-center"
						>
							{#each zips as zip}
								<button on:click={deleteZipOnClick} data-zip_name="{zip}">
									{zip}
								</button>
							{/each}
						</div>
					{/if}
				</div>
			{:else if zip_count == 0}
				<div>
					<h1>No Zips</h1>
				</div>
			{:else}
				<div>
					<h1>No connection...</h1>
				</div>
			{/if}
		</div>
	</section>

	{#if error_message}
		<section style="padding: 2rem;">
			{JSON.stringify(error_message)}
		</section>
	{/if}
</main>

<style>

	[css-flex]
	{
		display: flex;
	}
	[css-flex~="column"]
	{
		flex-direction: column;
	}
	[css-flex~="jc-center"]
	{
		justify-content: center;
	}
	[css-flex~="space-between"]
	{
		justify-content: space-between;
	}
	[css-flex~="wrap"]
	{
		flex-wrap: wrap;
	}
	[css-flex~="gap-1-2"]
	{
		gap: 1rem 2rem;
	}
	[css-flex~="margin-auto"]
	{
		margin: auto;
	}

	[css-font~="10-rem"]
	{
		font-size: 10rem;
	}

	[data-class-container]
	{
		opacity: 0;
	}

	main
	{
		display: flex;
		flex-wrap: wrap;
		position: relative;
		background-color: var(--background);
	}

	section
	{
		display: flex;
		width: 100%;
		height: fit-content;
		border: 1px solid var(--border);
		border-radius: var(--radius);
		margin: 2rem;
	}

	.container
	{
		display: flex;
		flex-direction: column;
		width: 100%;
		padding: 2rem 3rem;
	}

	button
	{
		display: block;
		width: fit-content;
		height: 2.5rem;
		padding: .5rem 1rem;
		color: var(--foreground);
		background-color: inherit;
		border: none;
		outline: none;
		border: 1px solid var(--border);
		border-radius: calc(var(--radius) - 2px);
		font-weight: 500;
		font-size: .875rem;
		line-height: 1.25rem;
		white-space: nowrap;
		cursor: pointer;
		transition: all ease-in-out 300ms;
	}

	button:hover:not(:disabled), button:focus:not(:disabled)
	{
		color: var(--accent-foreground);
		background-color: var(--accent);
	}

	h1, .h1
	{
		width: 100%;
		text-align: center;
		padding: 5rem 2rem;
	}

	.tree_view_container
	{
		width: fit-content;
		padding: 0 2rem;
		min-width: 312px;
	}

	.tree_view_container ul
	{
		margin-left: 20px;
	}

	.tree_view li 
	{
		list-style-type: none;
		margin: 10px 0 10px 10px;
		position: relative;
	}

	.tree_view li:before 
	{
		content: "";
		position: absolute;
		top: -10px;
		left: -20px;
		border-left: 1px solid var(--border);
		border-bottom: 1px solid var(--border);
		width: 20px;
		height: 15px;
	}

	.tree_view li:after 
	{
		position: absolute;
		content: "";
		top: 5px;
		left: -20px;
		border-left: 1px solid var(--border);
		border-top: 1px solid var(--border);
		width: 20px;
		height: 100%;
	}
	.tree_view li:last-child:after 
	{
		display: none;
	}

	.tree_view li span 
	{
		display: flex;
		align-items: end;
		gap: 0.5rem;
		border: 1px solid var(--border);
		padding: 10px;
		color: var(--muted-foreground);
		text-decoration: none;
	}

	.tree_view li span:hover, .tree_view li span:focus 
	{
		color: var(--accent-foreground);
		background-color: var(--accent);
	}

	.tree_view li span:hover + ul li span, .tree_view li span:focus + ul li span 
	{
		color: var(--accent-foreground);
		background-color: var(--accent);
	}

	.scan_icon
	{
		display: flex;
		width: 160px;
		height: 160px;
		overflow: hidden;
	}

	.scan_icon > i
	{
		opacity: 0;
		position: absolute;
		transition: all ease-in-out 300ms;
	}

	.scan_icon > i:last-child
	{
		transition-delay: 300ms;
	}

	.rotate_animation
	{
		animation: rotate_loop 5s linear infinite;
	}

	@keyframes rotate_loop
	{
		from 
		{
			transform: rotate(360deg);
		}
		to 
		{
			transform: rotate(0deg);
		}
	}

</style>