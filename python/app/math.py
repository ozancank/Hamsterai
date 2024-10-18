from IPython.display import display, Markdown
import markdown2

problem = r"""If $h(x)$ is a function whose domain is $[-8,8]$, and $g(x)=h\left(\frac{x}{2}\right)$, then the domain of $g(x)$ is an interval of what width?"""

html_content = markdown2.markdown(problem)

print(html_content)
